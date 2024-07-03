using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {

        [HttpGet]
        public void ListInteger()
        {
            for (int i = 0; i < 100000; i++) {
                Console.WriteLine($"This is the indicie -> {i}");
            }
        }



        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackground()
        {
            //Enqueue -> Adicionando em uma lista de espera
            BackgroundJob.Enqueue(() => ListInteger() );

            return Ok();
        } 
        
        
        [HttpPost]
        [Route("CreateScheduleJob")]
        public ActionResult CreateSchedule()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds( 25 );
           var dataDateTimeOffset = new DateTimeOffset( scheduleDateTime );

            //Para ser executado no tempo que o usuario escolher
            //Console.WriteLine("Sucess") -> esse primeiro parametro e o que sera executado
            //dataDateTimeOffset -> esse segundo parametro e o horario do agendamento que sera executado
            BackgroundJob.Schedule(() => Console.WriteLine("Sucess"), dataDateTimeOffset);

            return Ok();
        }
        
        
        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuation()
        {
           var scheduleDateTime = DateTime.UtcNow.AddSeconds( 25 );
           var dataDateTimeOffset = new DateTimeOffset( scheduleDateTime );

           //o metodo vai ser executado e o ID dele sera colocado dentro da variavel job1
           var job1 = BackgroundJob.Schedule(() => Console.WriteLine("job1"), dataDateTimeOffset);


            //ContinueJobWith -> depois que o job1 for realizado, esse job2 sera realizado tambem (tipo uma fileira)
            //Depois que o primeiro job1 executar que tem 25 segundos os outros jobs abaixo vai ser executados no mesmo tempo
            var job2 = BackgroundJob.ContinueJobWith(job1, () => Console.WriteLine("job2"));

            var job3 = BackgroundJob.ContinueJobWith(job2, () => Console.WriteLine("job3"));


            return Ok();
        }


        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurring()
        {
            //Executa a cada tempo, exemplo executa a cada 5 segundos etc..
            //RecurringJob1 -> id do job que queremos fazer isso
            //"* * * * *" -> Cron expression, nela vamos determina o tempo se e de 1 em 1 hora ou 5 em 5 minutos etc.. (ELA PRECISA TER ESPACOS ASSIM ENTRE OS *!!!)
            //"* * * * *" -> dessa forma esta a cada 1 minuto
            RecurringJob.AddOrUpdate("RecurringJob1", () => Console.WriteLine("Recurring job"), "* * * * *" );

            return Ok();
        }
    }
}
