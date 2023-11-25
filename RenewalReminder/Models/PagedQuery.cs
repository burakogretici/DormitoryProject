using KvsProject.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KvsProject.Models
{
    public class PagedQuery<T> where T : Entity
    {
        public int PageSize { get; set; } = 20;
        public int Page { get; set; } = 1;
        public List<string> Includes { get; set; }
        public List<Expression<Func<T, bool>>> Filters { get; set; }
        public List<Tuple<LambdaExpression, bool>> Orders { get; set; }
        public Expression<Func<T, T>> Select { get; set; }
    }
}
