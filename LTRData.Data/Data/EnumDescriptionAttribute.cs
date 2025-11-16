// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;

namespace LTRData.Data;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[AttributeUsage(AttributeTargets.Field)]
public class EnumDescriptionAttribute(string Description) : Attribute
{
    public string Description { get; set; } = Description;
}