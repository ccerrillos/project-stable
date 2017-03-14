using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ProjectStableLibrary {
	public class StableContextFactory {
		public static StableContext Build(string conStr) {
			var optionsBuilder = new DbContextOptionsBuilder<StableContext>();
			//optionsBuilder.
			optionsBuilder.UseMySQL(conStr);

			return new StableContext(optionsBuilder.Options);
		}
	}
	public class StableContext : DbContext {
		public StableContext(DbContextOptions<StableContext> options) : base(options) {
			//Model.
		}

		public DbSet<Date> dates {
			get;
			set;
		}
		public List<Date> Dates {
			get {
				return dates.ToList();
			}
		}
		public DbSet<Block> blocks {
			get;
			set;
		}
		public Dictionary<uint, Block> Blocks {
			get {
				Dictionary<uint, Block> blocks = new Dictionary<uint, Block>();
				foreach(Block b in this.blocks) {
					blocks.Add(b.block_id, b);
				}

				return blocks;
			}
		}
		public DbSet<Grade> grades {
			get;
			set;
		}
		public Dictionary<uint, Grade> Grades {
			get {
				Dictionary<uint, Grade> grades = new Dictionary<uint, Grade>();
				foreach(Grade g in this.grades) {
					grades.Add(g.grade_id, g);
				}

				return grades;
			}
		}
		public DbSet<House> houses {
			get;
			set;
		}
		public Dictionary<uint, House> Houses {
			get {
				Dictionary<uint, House> houses = new Dictionary<uint, House>();
				foreach(House h in this.houses) {
					houses.Add(h.house_id, h);
				}

				return houses;
			}
		}
		public DbSet<Location> locations {
			get;
			set;
		}
		public Dictionary<uint, Location> Locations {
			get {
				Dictionary<uint, Location> locations = new Dictionary<uint, Location>();
				foreach(Location l in this.locations) {
					locations.Add(l.location_id, l);
				}

				return locations;
			}
		}
		public DbSet<Presentation> presentations {
			get;
			set;
		}
		public Dictionary<uint, Presentation> Presentations {
			get {
				Dictionary<uint, Presentation> presentations = new Dictionary<uint, Presentation>();
				foreach(Presentation p in this.presentations) {
					presentations.Add(p.presentation_id, p);
				}

				return presentations;
			}
		}
		public DbSet<Viewer> viewers {
			get;
			set;
		}
		public Dictionary<uint, Viewer> Viewers {
			get {
				Dictionary<uint, Viewer> viewers = new Dictionary<uint, Viewer>();
				foreach(Viewer v in this.viewers) {
					viewers.Add(v.viewer_id, v);
				}

				return viewers;
			}
		}

	}
	
}
