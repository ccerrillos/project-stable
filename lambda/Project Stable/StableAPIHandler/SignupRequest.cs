
namespace ProjectStableLibrary {
	public class SignupRequest {
		public string first_name {
			get;
			set;
		}
		public string last_name {
			get;
			set;
		}
		public uint house {
			get;
			set;
		}
		public uint grade {
			get;
			set;
		}
	}
	public class SignupResponse {
		public bool status {
			get;
			set;
		}
		public uint viewer_id {
			get;
			set;
		}
		public string viewer_key {
			get;
			set;
		}
	}
}
