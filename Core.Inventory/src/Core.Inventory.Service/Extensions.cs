using System;
using Core.Inventory.Service.Dtos;
using Core.Inventory.Service.Entities;

namespace Core.Inventory.Service;

public static class Extensions
{
    public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
    {
        return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
    }
}
