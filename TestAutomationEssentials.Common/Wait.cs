using System;
using System.Linq.Expressions;

namespace TestAutomationEssentials.Common
{
	public class Wait
	{
		public static void Until(Expression<Func<bool>> conditionExpr, TimeSpan timeout)
		{
			if (conditionExpr == null)
				throw new ArgumentNullException("conditionExpr");

			var timeoutMessage = "The condition '" + conditionExpr + "' has not been met for " + timeout.ToSpokenString();

			Until(conditionExpr.Compile(), timeout, timeoutMessage);
		}

		public static void While(Expression<Func<bool>>  conditionExpr, TimeSpan timeout)
		{
			if (conditionExpr == null)
				throw new ArgumentNullException("conditionExpr");

			var timeoutMessage = "The condition '" + conditionExpr + "' is still true after " + timeout.ToSpokenString();
			var condition = conditionExpr.Compile();
			Until(() => !condition(), timeout, timeoutMessage);
		}

		private static void Until(Func<bool> condition, TimeSpan timeout, string timeoutMessage)
		{
			var conditionMet = IfNot(condition, timeout);
			if (!conditionMet)
				throw new TimeoutException(timeoutMessage);
		}

		/// <summary>
		/// Waits until the specified condition is met or until the specified period has passed, whichever comes first. This method doesn't throw TimeoutException.
		/// </summary>
		/// <param name="condition">The condition to evaluate</param>
		/// <param name="period">The period to wait for the condition</param>
		/// <returns>Whether the condition has been met</returns>
		/// <remarks>
		/// You should use it for non critical and possibly very short conditions that the polling may miss. After calling this method you should verify (or wait) 
		/// for a different condition that indicates that the operation has actually completed or not.
		/// </remarks>
		public static bool IfNot(Func<bool> condition, TimeSpan period)
		{
			var endTime = DateTime.Now + period;
			bool conditionMet;
			do
			{
				if (DateTime.Now > endTime)
				{
					return false;
				}

				conditionMet = condition();
			} while (!conditionMet);
			return true;
		}

		/// <summary>
		/// Waits while the specified condition is met or until the specified period has passed. This method doesn't throw TimeoutException.
		/// </summary>
		/// <param name="condition">The condition to evaluate</param>
		/// <param name="period">The period to wait for the condition</param>
		/// <returns>Whether the condition has been met</returns>
		/// <remarks>
		/// You should use it for non critical and possibly very short conditions that the polling may miss. After calling this method you should verify (or wait) 
		/// for a different condition that indicates that the operation has actually completed or not.
		/// </remarks>
		public static bool If(Func<bool> condition, TimeSpan period)
		{
			// TODO: move the main implementation to this method and change IfNot to call this one. This should resolve the double negation and simplify the code a little
			return IfNot(condition.Negate(), period);
		}
	}
}