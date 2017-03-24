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
		public Dictionary<uint, Block> blocks {
			get;
			set;
		}
		public List<Schedule> schedule {
			get;
			set;
		}
		public Dictionary<uint, Grade> grades {
			get;
			set;
		}
		public Dictionary<uint, House> houses {
			get;
			set;
		}
		public Dictionary<Schedule, List<Viewer>> viewers {
			get;
			set;
		}
		public override string ToString() {
			string response = "";
			response += "<style>@media print { .break {page-break-after: always;}} td { padding-right: 2em; }</style>";

			foreach(Schedule s in schedule) {
				// if(!viewers.ContainsKey(s))
				// 	continue;

				response += "<h4>" + presentationData.ToString() + " | " + locationData.location_name + " | " + blocks[s.block_id].block_name + "</h4>";
				response += $"<div>Viewer count: <b>{viewers[s].Count}</b></div>";
				response += "<table>";
				response += "<tr><th>Last</th><th>First</th>";
				response += "<th>Grade</th><th>House</th></tr>";
				foreach(Viewer v in viewers[s].OrderBy(thus => thus.last_name).ThenBy(thus => thus.first_name)) {
					response += $"<tr><td>{v.last_name}</td><td>{v.first_name}</td>";
					response += $"<td>{grades[v.grade_id].grade_name}</td><td>{houses[v.house_id].house_name}</td></tr>";
				}
				response += "</table>";
				response += "<div class=\"break\"></div>";
			}
			

			return response;
		}
	}
}