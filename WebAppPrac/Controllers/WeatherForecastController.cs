using Microsoft.AspNetCore.Mvc;

namespace WebAppPrac.Controllers // namespace == java �� package , �ֻ��� namespace �ؿ� ���� namespace ���� ����, ��Ű���� ����� ���� �뵵
{
	[ApiController] // @RestController
	[Route("[controller]")] // /weatherforecast url�� ���� Ŭ���̾�Ʈ�� http ��û�� ���� �� �ֵ��� ��
	public class WeatherForecastController : ControllerBase { // Controller�� ControllerBase�� ��ӹް� ����

		private static readonly string[] Summaries = new[] {
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

		// �ΰ�
		private readonly ILogger<WeatherForecastController> _logger; // Java������ final Ű���带, C#������ const�� readonly Ű���带 ����Ѵ�. => ���

		// Ŭ���� ������
		public WeatherForecastController(ILogger<WeatherForecastController> logger) {
			_logger = logger;
		}

		// @GetMapping '/WeatherForecast' ����
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


		// '/WeatherForecast/{idx}' ����
		[HttpGet("{idx:int}")]
		[ProducesResponseType(200, Type = typeof(object))] // �ش� ��û�� ���������� ó���Ǹ� 200 �����ڵ�� object ������ �����͸� �����Ѵٴ� �ǹ�
		[ProducesResponseType(404)] // �ش� ��û�� ���з� ó���Ǹ� 404�� ��ȯ�ȴ�.
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

		[HttpGet("requestThisURL")] // ���� url
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public string RequestThisURL()
		{
			return "!!!!";
		}

		[HttpGet("{str}")] // ������Ʈ�� �޾ƿ���
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

			return "�Խù� id : " + boardId + ", �ۼ��� : " + writer; 
		}
	}

}