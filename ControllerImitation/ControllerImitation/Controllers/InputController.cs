using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace ControllerImitation.Controllers
{
    public class ScenarioQuery
    {
        public long ScenarioId { get; set; }
        public ControllerLoginInfo Info { get; set; }
    }
    public class ScenarioInfo
    {
        public string Text { get; set; }
        public bool TurnedOn { get; set; }
        public long Id { get; set; }
    }
    public class ControllerLoginInfo
    {
        public long Id { get; set; }
        public string Password { get; set; }
    }
    [Produces("application/json")]
    [Route("api")]
    public class InputController:Controller
    {
        public static string ServerAdress;
        public static long Id;
        public static string Password;
        private static IEnumerable<ScenarioInfo> _scenarios = null;

        [HttpGet("ListUpdate")]
        public void ListInfoUpdate()
        {
            Console.WriteLine("Запрос на обновление списка сценариев получен");
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(ServerAdress + "/Scenarios",
                    new StringContent(
                        JsonConvert.SerializeObject(
                            new ControllerLoginInfo()
                            {
                                Id = Id,
                                Password = Password
                            }), Encoding.UTF8, "application/json"));
                var resp = response.Result.Content.ReadAsStringAsync().Result;
                _scenarios = JsonConvert.DeserializeObject<IEnumerable<ScenarioInfo>>(resp);
                Console.WriteLine("Сценарии обновлены");
            }
        }

        [HttpPost("Delete")]
        public void ScenarioDelete([FromBody]long id)
        {
            _scenarios = _scenarios.Where(x => x.Id != id);
            Console.WriteLine("удален сценарий " +id);
        }

        [HttpPost("Switch")]
        public void ScenarioSwitch([FromBody] KeyValuePair<long, bool> input)
        {
            var scenario = _scenarios.FirstOrDefault(x => x.Id == input.Key);
            if (scenario == null)
                throw new Exception("Сценарий не найден");
            scenario.TurnedOn = input.Value;
            ;
        }
        [HttpPost("Update")]
        public void ScenarioUpdate([FromBody] long id)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(ServerAdress + "/Scenario", new StringContent(JsonConvert.SerializeObject(
                    new ScenarioQuery()
                    {
                        ScenarioId = id,
                        Info = new ControllerLoginInfo()
                        {
                            Id = Id,
                            Password = Password
                        }
                    }), Encoding.UTF8, "application/json"));
                var newText = JsonConvert.DeserializeObject<string>(response.Result.Content.ReadAsStringAsync().Result);
                Console.WriteLine("Обновленный текст:");
                Console.WriteLine(newText);
                var prev = _scenarios.FirstOrDefault(x => x.Id == id);
                if (prev == null)
                    throw new Exception("Такой сценарий не найден");
                prev.Text = newText;

                Console.WriteLine("Обновлен сценарий " + id);

            }
        }

        [HttpGet]
        public IEnumerable<ScenarioInfo> ShowScenarios()
        {
            foreach (var a in _scenarios)
            {
                Console.WriteLine(a.Id+": "+a.TurnedOn);
                Console.WriteLine(a.Text);
            }

            return _scenarios;
        }
    }
}
