using System;

namespace ProjectStableLibrary {
	public struct Result {
		public bool status {
			get;
			set;
		}
		public string details {
			get;
			set;
		}
		public Result(Exception e) {
			status = false;
			details = e.InnerException != null ? e.InnerException.Message : e.Message;
		}
	}
}
