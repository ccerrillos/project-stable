using System.Collections.Generic;
using System.Linq;
using ProjectStableLibrary;

namespace StableAPIHandler {
	public class PrintOutput {
		public Presentation presentationData {
			get;
			set;
		}
		public Location locationData {
			get;
			set;
		}
		public List<Block> blocks {
			get;
			set;
		}
		public List<Schedule> schedule {
			get;
			set;
		}
		public Dictionary<Schedule, Viewer> viewers {
			get;
			set;
		}
		public override string ToString() {
			string response = "";
			response += "<style>@media print { .break {page-break-after: always;}}</style>";

			foreach(Schedule s in schedule) {
				// if(!viewers.ContainsKey(s))
				// 	continue;

				response += "<h3>" + presentationData.ToString() + " | " + locationData.location_name + "</h3>";

				response += "<div class=\"break\"></div>";
			}
			

			return response;
		}
	}
}