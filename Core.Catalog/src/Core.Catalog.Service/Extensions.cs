using System;
using Core.Catalog.Service.Dtos;
using Core.Catalog.Service.Entities;

namespace Core.Catalog.Service;

public static class Extensions
{
    public static ItemDto AsDto(this Item item)
    {
        return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
    }
}
