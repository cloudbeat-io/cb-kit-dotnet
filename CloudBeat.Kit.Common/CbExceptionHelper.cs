using CloudBeat.Kit.Common.Models;
using System;
using System.Linq;

namespace CloudBeat.Kit.Common
{
    public static class CbExceptionHelper
	{
		public const string ERROR_TYPE_WEBDRIVER = "WEBDRIVER_ERROR";
		public const string ERROR_TYPE_GENERAL = "GENERAL_ERROR";
		public const string ERROR_TYPE_ASSERT = "ASSERT_ERROR";
		public const string ERROR_TYPE_NUNIT = "NUNIT_ERROR";
		private const string ERROR_TYPE_UNKOWN = "UNKOWN_ERROR";
		private static readonly string[] ASSERT_EXCEPTIONS = { "AssertFailedException" };

		public static FailureResult GetFailureFromException(Exception e)
		{
			FailureResult failure = new FailureResult();

			failure.Type = GetFailureTypeByExceptionSource(e);
			failure.Subtype = e.InnerException?.GetType().Name ?? e.GetType().Name;
			failure.Message = e.Message;
			failure.Data = e.StackTrace;
			return failure;
		}
		public static string GetFailureTypeByExceptionSource(Exception e)
		{
			if (e.Source == "nunit.framework")
			{
				if (e.GetType().Name == "AssertionException")
					return ERROR_TYPE_ASSERT;
				else
					return ERROR_TYPE_NUNIT;
			}
			else if (e.Source == "WebDriver")
				return ERROR_TYPE_WEBDRIVER;
			else
			{
				string exceptionType = string.Empty;
				if (e.InnerException != null)
					exceptionType = e.InnerException.GetType().Name;
				else
					exceptionType = e.GetType().Name;
				if (ASSERT_EXCEPTIONS.Contains(exceptionType))
					return ERROR_TYPE_ASSERT;
			}
			return ERROR_TYPE_GENERAL;
		}
	}
}
