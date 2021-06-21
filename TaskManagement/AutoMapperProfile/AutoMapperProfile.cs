using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManagement.Areas.Admin.Models;
using TaskManagement.Models;

namespace TaskManagement
{
    public class AutoMapperProfile
    {
        public static IMapper Mapper { get; private set; }
        public static void Init()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskManage, TaskManageViewModel>();
                cfg.CreateMap<FileUpload, FileViewModel>();
                    //.ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                    //.ForMember(d => d.FileName, opt => opt.MapFrom(src => src.FileName))
                    //.ForMember(d => d.TaskId, opt => opt.MapFrom(src => src.TaskId));
            });
            Mapper = config.CreateMapper();
        }
    }
}