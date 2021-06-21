using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskManagement.Areas.Admin.Interfaces;

namespace TaskManagement.Areas.Admin.Services
{
    public class TestQuery : ITestQuery
    {
        private IMapper _mapper;
        public TestQuery(IMapper mapper)
        {
            _mapper = mapper;
        }
        public int Test()
        {
            return 1;
        }
    }
}