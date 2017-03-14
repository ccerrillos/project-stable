using System;

namespace ProjectStableLibrary {
	public struct Result {
		public bool status;
		public string details;
		public Result(Exception e) {
			status = false;
			details = e.InnerException != null ? e.InnerException.Message : e.Message;
		}
	}
}
