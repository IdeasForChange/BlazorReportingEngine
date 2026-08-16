using AutoMapper;

namespace Smbc.Risk.Core.Application.Extensions;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination> IgnoreAllUnmappedMembers<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        var destinationType = typeof(TDestination);
        foreach(var property in destinationType.GetProperties())
        {
            expression.ForMember(property.Name, opt => opt.Ignore());
        }
        return expression;
    }
}
