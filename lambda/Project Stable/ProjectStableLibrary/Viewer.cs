using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProjectStableLibrary {
	[Table("viewers")]
	public class Viewer {
		[Key]
		public uint viewer_id {
			get;
			set;
		}
		[MaxLength(16)]
		public string viewer_key {
			get;
			set;
		}
		[MaxLength(255)]
		public string first_name {
			get;
			set;
		}
		[MaxLength(255)]
		public string last_name {
			get;
			set;
		}
		public uint house_id {
			get;
			set;
		}
		// public virtual House House {
		// 	get;
		// 	set;
		// }
		public uint grade_id {
			get;
			set;
		}
		// public virtual Grade Grade {
		// 	get;
		// 	set;
		// }
		public SanitizedViewer Sanitize() {
			return new SanitizedViewer(this);
		}
	}
	public class SanitizedViewer {
		public SanitizedViewer(Viewer v) {
			viewer_id = v.viewer_id;
			first_name = v.first_name;
			last_name = v.last_name;
			house_id = v.house_id;
			grade_id = v.grade_id;
		}
		public uint viewer_id {
			get;
			set;
		}
		public string first_name {
			get;
			set;
		}
		public string last_name {
			get;
			set;
		}
		public uint house_id {
			get;
			set;
		}
		public uint grade_id {
			get;
			set;
		}
	}
}
