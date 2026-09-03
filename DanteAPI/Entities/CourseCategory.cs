using System;
namespace DanteAPI.Entities {
    public class CourseCategory {
        public int ID { get; set; }
        public string ImportID { get; set; }
        public string Name { get; set; }
        public int? ParentCourseCategoryID { get; set; }
        public CourseCategory ParentCourseCategory { get; set; }
        public bool ShowOnline { get; set; }
        public string FriendlyURL { get; set; }
        public string ShortDescription { get; set; }
        public string HTMLDescription { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string ImageBannerURL { get; set; }
        public References.File ImageFile { get; set; }
        public References.File ImageBannerFile { get; set; }
        public string CustomField { get; set; }
    }
}
