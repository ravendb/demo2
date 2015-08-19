﻿using System.Linq;
using System.Web.Http;
using DemoMethods.Indexes;
using Raven.Client;

namespace DemoMethods.Basic
{
    public partial class BasicController : ApiController
    {
        [HttpGet]
        public object MultiMapIndexingQuery()
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {

                return session.Query<IndexNameAndCountry.Result, IndexNameAndCountry>()
                    .Customize(x => x.WaitForNonStaleResults())
                    .Search(x => x.Country, "USA")
                    .Select(x => x.Name)
                    .ToList();
            }
        }
    }
}