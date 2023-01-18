using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MTCG.Http.Enums
{
	public class HttpStatusMessageEnum
	{
		public enum HttpStatusMessage
        {
			//client error responses
			[Description("400 Bad Request")]
			BadRequest = 400,
			[Description("401 Unauthorized")]
			Unauthorized = 401,
			[Description("403 Forbidden")]
			Forbidden = 403,
			[Description("404 Not Found")]
			NotFound = 404,

			//Server error response
			[Description("500 Internal Server Error")]
			ServerErr = 500,

			//success response
			[Description("200 OK")]
			Ok = 200
        }
	}
}
