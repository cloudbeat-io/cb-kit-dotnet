using CloudBeat.Kit.Common.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CloudBeat.Kit.Common
{
    public static class CbExceptionHelper
	{
		public const string ERROR_TYPE_WEBDRIVER = "WEBDRIVER_ERROR";
		public const string ERROR_TYPE_GENERAL = "GENERAL_ERROR";
		public const string ERROR_TYPE_HTTP = "HTTP_ERROR";
		public const string ERROR_TYPE_ASSERT = "ASSERT_ERROR";
		public const string ERROR_TYPE_NUNIT = "NUNIT_ERROR";
		public const string ERROR_TYPE_MSTEST = "MSTEST_ERROR";
		private const string ERROR_TYPE_UNKOWN = "UNKOWN_ERROR";
		private static readonly string[] ASSERT_EXCEPTIONS = { "AssertFailedException" };

		public static FailureResult GetFailureFromException(Exception e)
		{
			if (e == null)
				return null;

			FailureResult failure = new FailureResult();

			// unwrap original exception
			string stackTrace;
			if (e is FullStackException fse)
			{
				stackTrace = fse.FullStackTrace;
                e = fse.InnerException;
			}
			else
			{
				stackTrace = e.StackTrace;
            }

			failure.Type = GetFailureTypeByExceptionSource(e);
			failure.Subtype = e.InnerException?.GetType().Name ?? e.GetType().Name;
			failure.Message = e.Message;
			failure.Stacktrace = stackTrace;
			failure.Location = GetLocationFromStackTraceText(stackTrace);
            return failure;
		}

		private static readonly Regex StackTraceLocationRegex =
			new Regex(@"at\s+(.+?)\s+in\s+(.+):line\s+(\d+)", RegexOptions.Compiled);

		// Parses location out of the stack trace TEXT rather than walking the live Exception via
		// reflection (new StackTrace(e, true).GetFrames()) - that reflection-based approach turned out
		// to be unreliable in practice: GetFileName()/GetFileLineNumber() came back empty for frames
		// where the exception's own e.StackTrace string clearly had "in <file>:line <N>" info (verified
		// against a real MSTest AssertFailedException). The text is always there; parsing it is the
		// approach that's actually proven to work, for both a live Exception's .StackTrace and for
		// NUnit/MSTest result objects (TestContext.ResultAdapter, TestResult) that only ever expose
		// the stack trace as pre-formatted text in the first place, with no Exception object at all.
		public static string GetLocationFromStackTraceText(string stackTraceText)
		{
			if (string.IsNullOrEmpty(stackTraceText))
				return null;
			var match = StackTraceLocationRegex.Match(stackTraceText);
			if (!match.Success)
				return null;
			return $"{match.Groups[1].Value.Trim()}({match.Groups[2].Value.Trim()}:{match.Groups[3].Value.Trim()})";
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
			else if (e.Source == "Microsoft.VisualStudio.TestPlatform.TestFramework")
			{
				if (e.GetType().Name == "AssertFailedException")
					return ERROR_TYPE_ASSERT;
				else
					return ERROR_TYPE_MSTEST;
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
