
using System.Collections.Generic;

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
	public class SignupResponse : Viewer {
		public SignupResponse(Viewer v) {
			viewer_id = v.viewer_id;
			viewer_key = v.viewer_key;
			first_name = v.first_name;
			last_name = v.last_name;
			house_id = v.house_id;
			grade_id = v.grade_id;
		}
		public bool status {
			get;
			set;
		}
	}
	public class FinishSignupRequest {
		public string viewer_id {
			get;
			set;
		}
		public string Viewer_key {
			get;
			set;
		}
		public Dictionary<uint, Dictionary<uint, List<uint>>> data {
			get;
			set;
		}
		public bool status {
			get;
			set;
		}
	}
}
