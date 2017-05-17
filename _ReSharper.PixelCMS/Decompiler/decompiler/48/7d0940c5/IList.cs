// Type: System.Collections.Generic.IList`1
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
  /// <summary>
  /// Represents a collection of objects that can be individually accessed by index.
  /// </summary>
  /// <typeparam name="T">The type of elements in the list.</typeparam><filterpriority>1</filterpriority>
  [TypeDependency("System.SZArrayHelper")]
  [__DynamicallyInvokable]
  public interface IList<T> : ICollection<T>, IEnumerable<T>, IEnumerable
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// 
    /// <returns>
    /// The element at the specified index.
    /// </returns>
    /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
    [__DynamicallyInvokable]
    T this[int index] { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
    [__DynamicallyInvokable]
    int IndexOf(T item);

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
    [__DynamicallyInvokable]
    void Insert(int index, T item);

    /// <summary>
    /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
    [__DynamicallyInvokable]
    void RemoveAt(int index);
  }
}
