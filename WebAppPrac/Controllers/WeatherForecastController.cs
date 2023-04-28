using Microsoft.AspNetCore.Mvc;

namespace WebAppPrac.Controllers // namespace == java 의 package , 최상위 namespace 밑에 하위 namespace 정의 가능, 패키지별 블록을 묶는 용도
{
	[ApiController] // @RestController
	[Route("[controller]")] // /weatherforecast url을 통해 클라이언트가 http 요청을 만들 수 있도록 함
	public class WeatherForecastController : ControllerBase { // Controller가 ControllerBase를 상속받고 있음

		private static readonly string[] Summaries = new[] {
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

		// 로거
		private readonly ILogger<WeatherForecastController> _logger; // Java에서는 final 키워드를, C#에서는 const와 readonly 키워드를 사용한다. => 상수

		// 클래스 생성자
		public WeatherForecastController(ILogger<WeatherForecastController> logger) {
			_logger = logger;
		}

		// @GetMapping '/WeatherForecast' 매핑
		[HttpGet("GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get()
		{

			_logger.LogInformation("this is log test");

			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{

				Date = DateTime.Now.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]

			})
			.ToArray();
		}


		// '/WeatherForecast/{idx}' 매핑
		[HttpGet("{idx:int}")]
		[ProducesResponseType(200, Type = typeof(object))] // 해당 요청이 성공적으로 처리되면 200 응답코드와 object 형식의 데이터를 리턴한다는 의미
		[ProducesResponseType(404)] // 해당 요청이 실패로 처리되면 404가 반환된다.
		public string Get(int idx)
		{
			return Summaries[idx];
		}

		[HttpGet("test")]
		[ProducesResponseType(200)] 
		[ProducesResponseType(404)]  
		public string GetTest()
		{
			return "Test Request";
		}

		[HttpGet("requestThisURL")] // 매핑 url
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public string RequestThisURL()
		{
			return "!!!!";
		}

		[HttpGet("{str}")] // 쿼리스트링 받아오기
		[ProducesResponseType(200, Type = typeof(object))]
		[ProducesResponseType(404)]
		public string GetPrac(string str)
		{
			return str;
		}

		[HttpPost("PostTest")]
		public string PostPrac([FromBody] Board board) {

			var writer = board.Writer;
			var boardId = board.BoardId;

			return "게시물 id : " + boardId + ", 작성자 : " + writer; 
		}
	}

}