// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LTRData.Extensions.Collections;

/// <summary>
/// A System.Collections.Generic.List(Of T) extended with IDisposable implementation that disposes each
/// object in the list when the list is disposed.
/// </summary>
[ComVisible(false)]
public partial class DisposableList : DisposableList<IDisposable>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableList"/> class.
    /// </summary>
    public DisposableList() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableList"/> class with the specified capacity.
    /// </summary>
    public DisposableList(int capacity) : base(capacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableList"/> class that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    public DisposableList(IEnumerable<IDisposable> collection) : base(collection)
    {
    }
}

/// <summary>
/// A System.Collections.Generic.List(Of T) extended with IDisposable implementation that disposes each
/// object in the list when the list is disposed.
/// </summary>
/// <typeparam name="T">Type of elements in list. Type needs to implement IDisposable interface.</typeparam>
[ComVisible(false)]
public partial class DisposableList<T> : List<T>, IDisposable where T : IDisposable
{
    private bool disposedValue;    // To detect redundant calls

    /// <summary>
    /// Releases the resources used by the <see cref="DisposableList{T}"/>.
    /// </summary>
    /// <param name="disposing">
    /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: free managed resources when explicitly called
                foreach (var obj in this)
                    obj.Dispose();
            }
        }
        disposedValue = true;

        // TODO: free shared unmanaged resources

        Clear();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="DisposableList{T}"/>.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DisposableList{T}"/> class.
    /// Calls <see cref="Dispose(bool)"/> to release resources.
    /// </summary>
    ~DisposableList()
    {
        Dispose(false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableList{T}"/> class.
    /// </summary>
    public DisposableList() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableList{T}"/> class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the list can initially store.</param>
    public DisposableList(int capacity) : base(capacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DisposableList class that contains elements copied from the specified
    /// collection.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new DisposableList. Cannot be null.</param>
    public DisposableList(IEnumerable<T> collection) : base(collection)
    {
    }
}
