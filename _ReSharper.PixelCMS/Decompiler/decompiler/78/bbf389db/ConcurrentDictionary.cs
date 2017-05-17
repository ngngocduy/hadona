// Type: System.Collections.Concurrent.ConcurrentDictionary`2
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Collections.Concurrent
{
  /// <summary>
  /// Represents a thread-safe collection of key/value pairs that can be accessed by multiple threads concurrently.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam><typeparam name="TValue">The type of the values in the dictionary.</typeparam>
  [DebuggerDisplay("Count = {Count}")]
  [ComVisible(false)]
  [DebuggerTypeProxy(typeof (Mscorlib_DictionaryDebugView<,>))]
  [__DynamicallyInvokable]
  [Serializable]
  [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
  public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
  {
    private static readonly bool s_isValueWriteAtomic = ConcurrentDictionary<TKey, TValue>.IsValueWriteAtomic();
    [NonSerialized]
    private volatile ConcurrentDictionary<TKey, TValue>.Tables m_tables;
    internal IEqualityComparer<TKey> m_comparer;
    [NonSerialized]
    private readonly bool m_growLockArray;
    private int m_keyRehashCount;
    [NonSerialized]
    private int m_budget;
    private KeyValuePair<TKey, TValue>[] m_serializationArray;
    private int m_serializationConcurrencyLevel;
    private int m_serializationCapacity;
    private const int DEFAULT_CONCURRENCY_MULTIPLIER = 4;
    private const int DEFAULT_CAPACITY = 31;
    private const int MAX_LOCK_NUMBER = 1024;

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the key/value pair at the specified index.
    /// </returns>
    /// <param name="key">The key of the value to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is  null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> does not exist in the collection.</exception>
    [__DynamicallyInvokable]
    public TValue this[TKey key]
    {
      [__DynamicallyInvokable] get
      {
        TValue obj;
        if (!this.TryGetValue(key, out obj))
          throw new KeyNotFoundException();
        else
          return obj;
      }
      [__DynamicallyInvokable] set
      {
        if ((object) key == null)
          throw new ArgumentNullException("key");
        TValue resultingValue;
        this.TryAddInternal(key, value, true, true, out resultingValue);
      }
    }

    /// <summary>
    /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The number of key/value pairs contained in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </returns>
    /// <exception cref="T:System.OverflowException">The dictionary already contains the maximum number of elements (<see cref="F:System.Int32.MaxValue"/>).</exception>
    [__DynamicallyInvokable]
    public int Count
    {
      [__DynamicallyInvokable] get
      {
        int num = 0;
        int locksAcquired = 0;
        try
        {
          this.AcquireAllLocks(ref locksAcquired);
          for (int index = 0; index < this.m_tables.m_countPerLock.Length; ++index)
            num += this.m_tables.m_countPerLock[index];
        }
        finally
        {
          this.ReleaseLocks(0, locksAcquired);
        }
        return num;
      }
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> is empty.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> is empty; otherwise, false.
    /// </returns>
    [__DynamicallyInvokable]
    public bool IsEmpty
    {
      [__DynamicallyInvokable] get
      {
        int locksAcquired = 0;
        try
        {
          this.AcquireAllLocks(ref locksAcquired);
          for (int index = 0; index < this.m_tables.m_countPerLock.Length; ++index)
          {
            if (this.m_tables.m_countPerLock[index] != 0)
              return false;
          }
        }
        finally
        {
          this.ReleaseLocks(0, locksAcquired);
        }
        return true;
      }
    }

    /// <summary>
    /// Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A collection of keys in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public ICollection<TKey> Keys
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return (ICollection<TKey>) this.GetKeys();
      }
    }

    /// <summary>
    /// Gets a collection that contains the values in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A collection that contains the values in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public ICollection<TValue> Values
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return (ICollection<TValue>) this.GetValues();
      }
    }

    [__DynamicallyInvokable]
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool IDictionary.IsFixedSize
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool IDictionary.IsReadOnly
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    ICollection IDictionary.Keys
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return (ICollection) this.GetKeys();
      }
    }

    [__DynamicallyInvokable]
    ICollection IDictionary.Values
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return (ICollection) this.GetValues();
      }
    }

    [__DynamicallyInvokable]
    bool ICollection.IsSynchronized
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    object ICollection.SyncRoot
    {
      [__DynamicallyInvokable] get
      {
        throw new NotSupportedException(Environment.GetResourceString("ConcurrentCollection_SyncRoot_NotSupported"));
      }
    }

    static int DefaultConcurrencyLevel
    {
      private get
      {
        return 4 * PlatformHelper.ProcessorCount;
      }
    }

    static ConcurrentDictionary()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that is empty, has the default concurrency level, has the default initial capacity, and uses the default comparer for the key type.
    /// </summary>
    [__DynamicallyInvokable]
    public ConcurrentDictionary()
      : this(ConcurrentDictionary<TKey, TValue>.DefaultConcurrencyLevel, 31, true, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that is empty, has the specified concurrency level and capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> concurrently.</param><param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> can contain.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than 1.-or-<paramref name="capacity"/> is less than 0.</exception>
    [__DynamicallyInvokable]
    public ConcurrentDictionary(int concurrencyLevel, int capacity)
      : this(concurrencyLevel, capacity, false, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IEnumerable`1"/>, has the default concurrency level, has the default initial capacity, and uses the default comparer for the key type.
    /// </summary>
    /// <param name="collection">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> or any of its keys is  null.</exception><exception cref="T:System.ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
    [__DynamicallyInvokable]
    public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
      : this(collection, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that is empty, has the default concurrency level and capacity, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param><exception cref="T:System.ArgumentNullException"><paramref name="comparer"/> is null.</exception>
    [__DynamicallyInvokable]
    public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
      : this(ConcurrentDictionary<TKey, TValue>.DefaultConcurrencyLevel, 31, true, comparer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that contains elements copied from the specified <see cref="T:System.Collections.IEnumerable"/> has the default concurrency level, has the default initial capacity, and uses the specified  <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// <param name="collection">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> or <paramref name="comparer"/> is null.</exception>
    [__DynamicallyInvokable]
    public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
      : this(comparer)
    {
      if (collection == null)
        throw new ArgumentNullException("collection");
      this.InitializeFromCollection(collection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that contains elements copied from the specified <see cref="T:System.Collections.IEnumerable"/>, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> concurrently.</param><param name="collection">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> or <paramref name="comparer"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than 1.</exception><exception cref="T:System.ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
    [__DynamicallyInvokable]
    public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
      : this(concurrencyLevel, 31, false, comparer)
    {
      if (collection == null)
        throw new ArgumentNullException("collection");
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      this.InitializeFromCollection(collection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> class that is empty, has the specified concurrency level, has the specified initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> concurrently.</param><param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> can contain.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys.</param><exception cref="T:System.ArgumentNullException"><paramref name="comparer"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> or <paramref name="capacity"/> is less than 1.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer)
      : this(concurrencyLevel, capacity, false, comparer)
    {
    }

    internal ConcurrentDictionary(int concurrencyLevel, int capacity, bool growLockArray, IEqualityComparer<TKey> comparer)
    {
      if (concurrencyLevel < 1)
        throw new ArgumentOutOfRangeException("concurrencyLevel", this.GetResource("ConcurrentDictionary_ConcurrencyLevelMustBePositive"));
      if (capacity < 0)
        throw new ArgumentOutOfRangeException("capacity", this.GetResource("ConcurrentDictionary_CapacityMustNotBeNegative"));
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      if (capacity < concurrencyLevel)
        capacity = concurrencyLevel;
      object[] locks = new object[concurrencyLevel];
      for (int index = 0; index < locks.Length; ++index)
        locks[index] = new object();
      int[] countPerLock = new int[locks.Length];
      ConcurrentDictionary<TKey, TValue>.Node[] buckets = new ConcurrentDictionary<TKey, TValue>.Node[capacity];
      this.m_tables = new ConcurrentDictionary<TKey, TValue>.Tables(buckets, locks, countPerLock, comparer);
      this.m_growLockArray = growLockArray;
      this.m_budget = buckets.Length / locks.Length;
    }

    private static bool IsValueWriteAtomic()
    {
      Type type = typeof (TValue);
      bool flag = type.IsClass || type == typeof (bool) || (type == typeof (char) || type == typeof (byte)) || (type == typeof (sbyte) || type == typeof (short) || (type == typeof (ushort) || type == typeof (int))) || type == typeof (uint) || type == typeof (float);
      if (!flag && IntPtr.Size == 8)
        flag = ((flag ? 1 : 0) | (type == typeof (double) ? 1 : (type == typeof (long) ? 1 : 0))) != 0;
      return flag;
    }

    private void InitializeFromCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in collection)
      {
        if ((object) keyValuePair.Key == null)
          throw new ArgumentNullException("key");
        TValue resultingValue;
        if (!this.TryAddInternal(keyValuePair.Key, keyValuePair.Value, false, false, out resultingValue))
          throw new ArgumentException(this.GetResource("ConcurrentDictionary_SourceContainsDuplicateKeys"));
      }
      if (this.m_budget != 0)
        return;
      this.m_budget = this.m_tables.m_buckets.Length / this.m_tables.m_locks.Length;
    }

    /// <summary>
    /// Attempts to add the specified key and value to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if the key/value pair was added to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> successfully; false if the key already exists.
    /// </returns>
    /// <param name="key">The key of the element to add.</param><param name="value">The value of the element to add. The value can be  null for reference types.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is  null.</exception><exception cref="T:System.OverflowException">The dictionary already contains the maximum number of elements (<see cref="F:System.Int32.MaxValue"/>).</exception>
    [__DynamicallyInvokable]
    public bool TryAdd(TKey key, TValue value)
    {
      if ((object) key == null)
      {
        throw new ArgumentNullException("key");
      }
      else
      {
        TValue resultingValue;
        return this.TryAddInternal(key, value, false, true, out resultingValue);
      }
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> contains the specified key.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> contains an element with the specified key; otherwise, false.
    /// </returns>
    /// <param name="key">The key to locate in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
    [__DynamicallyInvokable]
    public bool ContainsKey(TKey key)
    {
      if ((object) key == null)
      {
        throw new ArgumentNullException("key");
      }
      else
      {
        TValue obj;
        return this.TryGetValue(key, out obj);
      }
    }

    /// <summary>
    /// Attempts to remove and return the value that has the specified key from the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if the object was removed successfully; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the element to remove and return.</param><param name="value">When this method returns, contains the object removed from the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>, or the default value of  the TValue type if <paramref name="key"/> does not exist. </param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is  null.</exception>
    [__DynamicallyInvokable]
    public bool TryRemove(TKey key, out TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      else
        return this.TryRemoveInternal(key, out value, false, default (TValue));
    }

    private bool TryRemoveInternal(TKey key, out TValue value, bool matchValue, TValue oldValue)
    {
label_0:
      ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
      IEqualityComparer<TKey> equalityComparer = tables.m_comparer;
      int bucketNo;
      int lockNo;
      this.GetBucketAndLockNo(equalityComparer.GetHashCode(key), out bucketNo, out lockNo, tables.m_buckets.Length, tables.m_locks.Length);
      lock (tables.m_locks[lockNo])
      {
        if (tables == this.m_tables)
        {
          ConcurrentDictionary<TKey, TValue>.Node local_4 = (ConcurrentDictionary<TKey, TValue>.Node) null;
          for (ConcurrentDictionary<TKey, TValue>.Node local_5 = tables.m_buckets[bucketNo]; local_5 != null; local_5 = local_5.m_next)
          {
            if (equalityComparer.Equals(local_5.m_key, key))
            {
              if (matchValue && !EqualityComparer<TValue>.Default.Equals(oldValue, local_5.m_value))
              {
                value = default (TValue);
                return false;
              }
              else
              {
                if (local_4 == null)
                  Volatile.Write<ConcurrentDictionary<TKey, TValue>.Node>(ref tables.m_buckets[bucketNo], local_5.m_next);
                else
                  local_4.m_next = local_5.m_next;
                value = local_5.m_value;
                --tables.m_countPerLock[lockNo];
                return true;
              }
            }
            else
              local_4 = local_5;
          }
        }
        else
          goto label_0;
      }
      value = default (TValue);
      return false;
    }

    /// <summary>
    /// Attempts to get the value associated with the specified key from the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if the key was found in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the value to get.</param><param name="value">When this method returns, contains the object from the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> that has the specified key, or the default value of , if the operation failed.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is  null.</exception>
    [__DynamicallyInvokable]
    public bool TryGetValue(TKey key, out TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
      IEqualityComparer<TKey> equalityComparer = tables.m_comparer;
      int bucketNo;
      int lockNo;
      this.GetBucketAndLockNo(equalityComparer.GetHashCode(key), out bucketNo, out lockNo, tables.m_buckets.Length, tables.m_locks.Length);
      for (ConcurrentDictionary<TKey, TValue>.Node node = Volatile.Read<ConcurrentDictionary<TKey, TValue>.Node>(ref tables.m_buckets[bucketNo]); node != null; node = node.m_next)
      {
        if (equalityComparer.Equals(node.m_key, key))
        {
          value = node.m_value;
          return true;
        }
      }
      value = default (TValue);
      return false;
    }

    /// <summary>
    /// Compares the existing value for the specified key with a specified value, and if they are equal, updates the key with a third value.
    /// </summary>
    /// 
    /// <returns>
    /// true if the value with <paramref name="key"/> was equal to <paramref name="comparisonValue"/> and was replaced with <paramref name="newValue"/>; otherwise, false.
    /// </returns>
    /// <param name="key">The key whose value is compared with <paramref name="comparisonValue"/> and possibly replaced.</param><param name="newValue">The value that replaces the value of the element that has the specified <paramref name="key"/> if the comparison results in equality.</param><param name="comparisonValue">The value that is compared to the value of the element that has the specified <paramref name="key"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
    [__DynamicallyInvokable]
    public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      IEqualityComparer<TValue> equalityComparer1 = (IEqualityComparer<TValue>) EqualityComparer<TValue>.Default;
label_3:
      ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
      IEqualityComparer<TKey> equalityComparer2 = tables.m_comparer;
      int hashCode = equalityComparer2.GetHashCode(key);
      int bucketNo;
      int lockNo;
      this.GetBucketAndLockNo(hashCode, out bucketNo, out lockNo, tables.m_buckets.Length, tables.m_locks.Length);
      lock (tables.m_locks[lockNo])
      {
        if (tables == this.m_tables)
        {
          ConcurrentDictionary<TKey, TValue>.Node local_6 = (ConcurrentDictionary<TKey, TValue>.Node) null;
          for (ConcurrentDictionary<TKey, TValue>.Node local_7 = tables.m_buckets[bucketNo]; local_7 != null; local_7 = local_7.m_next)
          {
            if (equalityComparer2.Equals(local_7.m_key, key))
            {
              if (!equalityComparer1.Equals(local_7.m_value, comparisonValue))
                return false;
              if (ConcurrentDictionary<TKey, TValue>.s_isValueWriteAtomic)
              {
                local_7.m_value = newValue;
              }
              else
              {
                ConcurrentDictionary<TKey, TValue>.Node local_8 = new ConcurrentDictionary<TKey, TValue>.Node(local_7.m_key, newValue, hashCode, local_7.m_next);
                if (local_6 == null)
                  tables.m_buckets[bucketNo] = local_8;
                else
                  local_6.m_next = local_8;
              }
              return true;
            }
            else
              local_6 = local_7;
          }
          return false;
        }
        else
          goto label_3;
      }
    }

    /// <summary>
    /// Removes all keys and values from the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </summary>
    [__DynamicallyInvokable]
    public void Clear()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        ConcurrentDictionary<TKey, TValue>.Tables tables = new ConcurrentDictionary<TKey, TValue>.Tables(new ConcurrentDictionary<TKey, TValue>.Node[31], this.m_tables.m_locks, new int[this.m_tables.m_countPerLock.Length], this.m_tables.m_comparer);
        this.m_tables = tables;
        this.m_budget = Math.Max(1, tables.m_buckets.Length / tables.m_locks.Length);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    [__DynamicallyInvokable]
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index", this.GetResource("ConcurrentDictionary_IndexIsNegative"));
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        int num = 0;
        for (int index1 = 0; index1 < this.m_tables.m_locks.Length && num >= 0; ++index1)
          num += this.m_tables.m_countPerLock[index1];
        if (array.Length - num < index || num < 0)
          throw new ArgumentException(this.GetResource("ConcurrentDictionary_ArrayNotLargeEnough"));
        this.CopyToPairs(array, index);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    /// <summary>
    /// Copies the key and value pairs stored in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> to a new array.
    /// </summary>
    /// 
    /// <returns>
    /// A new array containing a snapshot of key and value pairs copied from the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public KeyValuePair<TKey, TValue>[] ToArray()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        int length = 0;
        int index = 0;
        while (index < this.m_tables.m_locks.Length)
        {
          checked { length += this.m_tables.m_countPerLock[index]; }
          checked { ++index; }
        }
        KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[length];
        this.CopyToPairs(array, 0);
        return array;
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private void CopyToPairs(KeyValuePair<TKey, TValue>[] array, int index)
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node node1 in this.m_tables.m_buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node node2 = node1; node2 != null; node2 = node2.m_next)
        {
          array[index] = new KeyValuePair<TKey, TValue>(node2.m_key, node2.m_value);
          ++index;
        }
      }
    }

    private void CopyToEntries(DictionaryEntry[] array, int index)
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node node1 in this.m_tables.m_buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node node2 = node1; node2 != null; node2 = node2.m_next)
        {
          array[index] = new DictionaryEntry((object) node2.m_key, (object) node2.m_value);
          ++index;
        }
      }
    }

    private void CopyToObjects(object[] array, int index)
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node node1 in this.m_tables.m_buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node node2 = node1; node2 != null; node2 = node2.m_next)
        {
          array[index] = (object) new KeyValuePair<TKey, TValue>(node2.m_key, node2.m_value);
          ++index;
        }
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </summary>
    /// 
    /// <returns>
    /// An enumerator for the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node location in this.m_tables.m_buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node current = Volatile.Read<ConcurrentDictionary<TKey, TValue>.Node>(ref location); current != null; current = current.m_next)
          yield return new KeyValuePair<TKey, TValue>(current.m_key, current.m_value);
      }
    }

    private bool TryAddInternal(TKey key, TValue value, bool updateIfExists, bool acquireLock, out TValue resultingValue)
    {
label_0:
      ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
      IEqualityComparer<TKey> equalityComparer = tables.m_comparer;
      int hashCode = equalityComparer.GetHashCode(key);
      int bucketNo;
      int lockNo;
      this.GetBucketAndLockNo(hashCode, out bucketNo, out lockNo, tables.m_buckets.Length, tables.m_locks.Length);
      bool flag1 = false;
      bool lockTaken = false;
      bool flag2 = false;
      try
      {
        if (acquireLock)
          Monitor.Enter(tables.m_locks[lockNo], ref lockTaken);
        if (tables == this.m_tables)
        {
          int num = 0;
          ConcurrentDictionary<TKey, TValue>.Node node1 = (ConcurrentDictionary<TKey, TValue>.Node) null;
          for (ConcurrentDictionary<TKey, TValue>.Node node2 = tables.m_buckets[bucketNo]; node2 != null; node2 = node2.m_next)
          {
            if (equalityComparer.Equals(node2.m_key, key))
            {
              if (updateIfExists)
              {
                if (ConcurrentDictionary<TKey, TValue>.s_isValueWriteAtomic)
                {
                  node2.m_value = value;
                }
                else
                {
                  ConcurrentDictionary<TKey, TValue>.Node node3 = new ConcurrentDictionary<TKey, TValue>.Node(node2.m_key, value, hashCode, node2.m_next);
                  if (node1 == null)
                    tables.m_buckets[bucketNo] = node3;
                  else
                    node1.m_next = node3;
                }
                resultingValue = value;
              }
              else
                resultingValue = node2.m_value;
              return false;
            }
            else
            {
              node1 = node2;
              ++num;
            }
          }
          if (num > 100 && HashHelpers.IsWellKnownEqualityComparer((object) equalityComparer))
          {
            flag1 = true;
            flag2 = true;
          }
          Volatile.Write<ConcurrentDictionary<TKey, TValue>.Node>(ref tables.m_buckets[bucketNo], new ConcurrentDictionary<TKey, TValue>.Node(key, value, hashCode, tables.m_buckets[bucketNo]));
          checked { ++tables.m_countPerLock[lockNo]; }
          if (tables.m_countPerLock[lockNo] > this.m_budget)
            flag1 = true;
        }
        else
          goto label_0;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(tables.m_locks[lockNo]);
      }
      if (flag1)
      {
        if (flag2)
          this.GrowTable(tables, (IEqualityComparer<TKey>) HashHelpers.GetRandomizedEqualityComparer((object) equalityComparer), true, this.m_keyRehashCount);
        else
          this.GrowTable(tables, tables.m_comparer, false, this.m_keyRehashCount);
      }
      resultingValue = value;
      return true;
    }

    /// <summary>
    /// Adds a key/value pair to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> by using the specified function, if the key does not already exist.
    /// </summary>
    /// 
    /// <returns>
    /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.
    /// </returns>
    /// <param name="key">The key of the element to add.</param><param name="valueFactory">The function used to generate a value for the key</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> or <paramref name="valueFactory"/> is null.</exception><exception cref="T:System.OverflowException">The dictionary already contains the maximum number of elements (<see cref="F:System.Int32.MaxValue"/>).</exception>
    [__DynamicallyInvokable]
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      if (valueFactory == null)
        throw new ArgumentNullException("valueFactory");
      TValue resultingValue;
      if (this.TryGetValue(key, out resultingValue))
        return resultingValue;
      this.TryAddInternal(key, valueFactory(key), false, true, out resultingValue);
      return resultingValue;
    }

    /// <summary>
    /// Adds a key/value pair to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> if the key does not already exist.
    /// </summary>
    /// 
    /// <returns>
    /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value if the key was not in the dictionary.
    /// </returns>
    /// <param name="key">The key of the element to add.</param><param name="value">the value to be added, if the key does not already exist</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.OverflowException">The dictionary already contains the maximum number of elements (<see cref="F:System.Int32.MaxValue"/>).</exception>
    [__DynamicallyInvokable]
    public TValue GetOrAdd(TKey key, TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      TValue resultingValue;
      this.TryAddInternal(key, value, false, true, out resultingValue);
      return resultingValue;
    }

    /// <summary>
    /// Uses the specified functions to add a key/value pair to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> if the key does not already exist, or to update a key/value pair in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> if the key already exists.
    /// </summary>
    /// 
    /// <returns>
    /// The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or the result of updateValueFactory (if the key was present).
    /// </returns>
    /// <param name="key">The key to be added or whose value should be updated</param><param name="addValueFactory">The function used to generate a value for an absent key</param><param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/>, <paramref name="addValueFactory"/>, or <paramref name="updateValueFactory"/> is null.</exception><exception cref="T:System.OverflowException">The dictionary already contains the maximum number of elements (<see cref="F:System.Int32.MaxValue"/>).</exception>
    [__DynamicallyInvokable]
    public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      if (addValueFactory == null)
        throw new ArgumentNullException("addValueFactory");
      if (updateValueFactory == null)
        throw new ArgumentNullException("updateValueFactory");
      TValue comparisonValue;
      TValue newValue;
      do
      {
        while (!this.TryGetValue(key, out comparisonValue))
        {
          TValue obj = addValueFactory(key);
          TValue resultingValue;
          if (this.TryAddInternal(key, obj, false, true, out resultingValue))
            return resultingValue;
        }
        newValue = updateValueFactory(key, comparisonValue);
      }
      while (!this.TryUpdate(key, newValue, comparisonValue));
      return newValue;
    }

    /// <summary>
    /// Adds a key/value pair to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> if the key does not already exist, or updates a key/value pair in the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2"/> by using the specified function if the key already exists.
    /// </summary>
    /// 
    /// <returns>
    /// The new value for the key. This will be either be addValue (if the key was absent) or the result of updateValueFactory (if the key was present).
    /// </returns>
    /// <param name="key">The key to be added or whose value should be updated</param><param name="addValue">The value to be added for an absent key</param><param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> or <paramref name="updateValueFactory"/> is null.</exception><exception cref="T:System.OverflowException">The dictionary already contains the maximum number of elements (<see cref="F:System.Int32.MaxValue"/>).</exception>
    [__DynamicallyInvokable]
    public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
    {
      if ((object) key == null)
        throw new ArgumentNullException("key");
      if (updateValueFactory == null)
        throw new ArgumentNullException("updateValueFactory");
      TValue comparisonValue;
      TValue newValue;
      do
      {
        while (!this.TryGetValue(key, out comparisonValue))
        {
          TValue resultingValue;
          if (this.TryAddInternal(key, addValue, false, true, out resultingValue))
            return resultingValue;
        }
        newValue = updateValueFactory(key, comparisonValue);
      }
      while (!this.TryUpdate(key, newValue, comparisonValue));
      return newValue;
    }

    [__DynamicallyInvokable]
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      if (!this.TryAdd(key, value))
        throw new ArgumentException(this.GetResource("ConcurrentDictionary_KeyAlreadyExisted"));
    }

    [__DynamicallyInvokable]
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      TValue obj;
      return this.TryRemove(key, out obj);
    }

    [__DynamicallyInvokable]
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
    {
      this.Add(keyValuePair.Key, keyValuePair.Value);
    }

    [__DynamicallyInvokable]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
    {
      TValue x;
      if (!this.TryGetValue(keyValuePair.Key, out x))
        return false;
      else
        return EqualityComparer<TValue>.Default.Equals(x, keyValuePair.Value);
    }

    [__DynamicallyInvokable]
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
      if ((object) keyValuePair.Key == null)
      {
        throw new ArgumentNullException(this.GetResource("ConcurrentDictionary_ItemKeyIsNull"));
      }
      else
      {
        TValue obj;
        return this.TryRemoveInternal(keyValuePair.Key, out obj, true, keyValuePair.Value);
      }
    }

    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    [__DynamicallyInvokable]
    void IDictionary.Add(object key, object value)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (!(key is TKey))
        throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfKeyIncorrect"));
      TValue obj;
      try
      {
        obj = (TValue) value;
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfValueIncorrect"));
      }
      this.Add((TKey) key, obj);
    }

    [__DynamicallyInvokable]
    bool IDictionary.Contains(object key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (key is TKey)
        return this.ContainsKey((TKey) key);
      else
        return false;
    }

    [__DynamicallyInvokable]
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new ConcurrentDictionary<TKey, TValue>.DictionaryEnumerator(this);
    }

    [__DynamicallyInvokable]
    void IDictionary.Remove(object key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (!(key is TKey))
        return;
      TValue obj;
      this.TryRemove((TKey) key, out obj);
    }

    [__DynamicallyInvokable]
    object IDictionary.get_Item(object key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      TValue obj;
      if (key is TKey && this.TryGetValue((TKey) key, out obj))
        return (object) obj;
      else
        return (object) null;
    }

    [__DynamicallyInvokable]
    void IDictionary.set_Item(object key, object value)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (!(key is TKey))
        throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfKeyIncorrect"));
      if (!(value is TValue))
        throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfValueIncorrect"));
      this[(TKey) key] = (TValue) value;
    }

    [__DynamicallyInvokable]
    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0)
        throw new ArgumentOutOfRangeException("index", this.GetResource("ConcurrentDictionary_IndexIsNegative"));
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
        int num = 0;
        for (int index1 = 0; index1 < tables.m_locks.Length && num >= 0; ++index1)
          num += tables.m_countPerLock[index1];
        if (array.Length - num < index || num < 0)
          throw new ArgumentException(this.GetResource("ConcurrentDictionary_ArrayNotLargeEnough"));
        KeyValuePair<TKey, TValue>[] array1 = array as KeyValuePair<TKey, TValue>[];
        if (array1 != null)
        {
          this.CopyToPairs(array1, index);
        }
        else
        {
          DictionaryEntry[] array2 = array as DictionaryEntry[];
          if (array2 != null)
          {
            this.CopyToEntries(array2, index);
          }
          else
          {
            object[] array3 = array as object[];
            if (array3 == null)
              throw new ArgumentException(this.GetResource("ConcurrentDictionary_ArrayIncorrectType"), "array");
            this.CopyToObjects(array3, index);
          }
        }
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private void GrowTable(ConcurrentDictionary<TKey, TValue>.Tables tables, IEqualityComparer<TKey> newComparer, bool regenerateHashKeys, int rehashCount)
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireLocks(0, 1, ref locksAcquired);
        if (regenerateHashKeys && rehashCount == this.m_keyRehashCount)
        {
          tables = this.m_tables;
        }
        else
        {
          if (tables != this.m_tables)
            return;
          long num = 0L;
          for (int index = 0; index < tables.m_countPerLock.Length; ++index)
            num += (long) tables.m_countPerLock[index];
          if (num < (long) (tables.m_buckets.Length / 4))
          {
            this.m_budget = 2 * this.m_budget;
            if (this.m_budget >= 0)
              return;
            this.m_budget = int.MaxValue;
            return;
          }
        }
        int length1 = 0;
        bool flag = false;
        try
        {
          length1 = checked (tables.m_buckets.Length * 2 + 1);
          while (length1 % 3 == 0 || length1 % 5 == 0 || length1 % 7 == 0)
            checked { length1 += 2; }
          if (length1 > 2146435071)
            flag = true;
        }
        catch (OverflowException ex)
        {
          flag = true;
        }
        if (flag)
        {
          length1 = 2146435071;
          this.m_budget = int.MaxValue;
        }
        this.AcquireLocks(1, tables.m_locks.Length, ref locksAcquired);
        object[] locks = tables.m_locks;
        if (this.m_growLockArray && tables.m_locks.Length < 1024)
        {
          locks = new object[tables.m_locks.Length * 2];
          Array.Copy((Array) tables.m_locks, (Array) locks, tables.m_locks.Length);
          for (int length2 = tables.m_locks.Length; length2 < locks.Length; ++length2)
            locks[length2] = new object();
        }
        ConcurrentDictionary<TKey, TValue>.Node[] buckets = new ConcurrentDictionary<TKey, TValue>.Node[length1];
        int[] countPerLock = new int[locks.Length];
        for (int index = 0; index < tables.m_buckets.Length; ++index)
        {
          for (ConcurrentDictionary<TKey, TValue>.Node node1 = tables.m_buckets[index]; node1 != null; {
            ConcurrentDictionary<TKey, TValue>.Node node2;
            node1 = node2;
          }
          )
          {
            node2 = node1.m_next;
            int hashcode = node1.m_hashcode;
            if (regenerateHashKeys)
              hashcode = newComparer.GetHashCode(node1.m_key);
            int bucketNo;
            int lockNo;
            this.GetBucketAndLockNo(hashcode, out bucketNo, out lockNo, buckets.Length, locks.Length);
            buckets[bucketNo] = new ConcurrentDictionary<TKey, TValue>.Node(node1.m_key, node1.m_value, hashcode, buckets[bucketNo]);
            checked { ++countPerLock[lockNo]; }
          }
        }
        if (regenerateHashKeys)
          ++this.m_keyRehashCount;
        this.m_budget = Math.Max(1, buckets.Length / locks.Length);
        this.m_tables = new ConcurrentDictionary<TKey, TValue>.Tables(buckets, locks, countPerLock, newComparer);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private void GetBucketAndLockNo(int hashcode, out int bucketNo, out int lockNo, int bucketCount, int lockCount)
    {
      bucketNo = (hashcode & int.MaxValue) % bucketCount;
      lockNo = bucketNo % lockCount;
    }

    private void AcquireAllLocks(ref int locksAcquired)
    {
      if (CDSCollectionETWBCLProvider.Log.IsEnabled())
        CDSCollectionETWBCLProvider.Log.ConcurrentDictionary_AcquiringAllLocks(this.m_tables.m_buckets.Length);
      this.AcquireLocks(0, 1, ref locksAcquired);
      this.AcquireLocks(1, this.m_tables.m_locks.Length, ref locksAcquired);
    }

    private void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
    {
      object[] objArray = this.m_tables.m_locks;
      for (int index = fromInclusive; index < toExclusive; ++index)
      {
        bool lockTaken = false;
        try
        {
          Monitor.Enter(objArray[index], ref lockTaken);
        }
        finally
        {
          if (lockTaken)
            ++locksAcquired;
        }
      }
    }

    private void ReleaseLocks(int fromInclusive, int toExclusive)
    {
      for (int index = fromInclusive; index < toExclusive; ++index)
        Monitor.Exit(this.m_tables.m_locks[index]);
    }

    private ReadOnlyCollection<TKey> GetKeys()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        List<TKey> list = new List<TKey>();
        for (int index = 0; index < this.m_tables.m_buckets.Length; ++index)
        {
          for (ConcurrentDictionary<TKey, TValue>.Node node = this.m_tables.m_buckets[index]; node != null; node = node.m_next)
            list.Add(node.m_key);
        }
        return new ReadOnlyCollection<TKey>((IList<TKey>) list);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private ReadOnlyCollection<TValue> GetValues()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        List<TValue> list = new List<TValue>();
        for (int index = 0; index < this.m_tables.m_buckets.Length; ++index)
        {
          for (ConcurrentDictionary<TKey, TValue>.Node node = this.m_tables.m_buckets[index]; node != null; node = node.m_next)
            list.Add(node.m_value);
        }
        return new ReadOnlyCollection<TValue>((IList<TValue>) list);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    [Conditional("DEBUG")]
    private void Assert(bool condition)
    {
    }

    private string GetResource(string key)
    {
      return Environment.GetResourceString(key);
    }

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
      this.m_serializationArray = this.ToArray();
      this.m_serializationConcurrencyLevel = tables.m_locks.Length;
      this.m_serializationCapacity = tables.m_buckets.Length;
      this.m_comparer = (IEqualityComparer<TKey>) HashHelpers.GetEqualityComparerForSerialization((object) tables.m_comparer);
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      KeyValuePair<TKey, TValue>[] keyValuePairArray = this.m_serializationArray;
      ConcurrentDictionary<TKey, TValue>.Node[] buckets = new ConcurrentDictionary<TKey, TValue>.Node[this.m_serializationCapacity];
      int[] countPerLock = new int[this.m_serializationConcurrencyLevel];
      object[] locks = new object[this.m_serializationConcurrencyLevel];
      for (int index = 0; index < locks.Length; ++index)
        locks[index] = new object();
      this.m_tables = new ConcurrentDictionary<TKey, TValue>.Tables(buckets, locks, countPerLock, this.m_comparer);
      this.InitializeFromCollection((IEnumerable<KeyValuePair<TKey, TValue>>) keyValuePairArray);
      this.m_serializationArray = (KeyValuePair<TKey, TValue>[]) null;
    }

    private class Tables
    {
      internal readonly ConcurrentDictionary<TKey, TValue>.Node[] m_buckets;
      internal readonly object[] m_locks;
      internal volatile int[] m_countPerLock;
      internal readonly IEqualityComparer<TKey> m_comparer;

      internal Tables(ConcurrentDictionary<TKey, TValue>.Node[] buckets, object[] locks, int[] countPerLock, IEqualityComparer<TKey> comparer)
      {
        this.m_buckets = buckets;
        this.m_locks = locks;
        this.m_countPerLock = countPerLock;
        this.m_comparer = comparer;
      }
    }

    private class Node
    {
      internal TKey m_key;
      internal TValue m_value;
      internal volatile ConcurrentDictionary<TKey, TValue>.Node m_next;
      internal int m_hashcode;

      internal Node(TKey key, TValue value, int hashcode, ConcurrentDictionary<TKey, TValue>.Node next)
      {
        this.m_key = key;
        this.m_value = value;
        this.m_next = next;
        this.m_hashcode = hashcode;
      }
    }

    private class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
    {
      private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

      public DictionaryEntry Entry
      {
        get
        {
          return new DictionaryEntry((object) this.m_enumerator.Current.Key, (object) this.m_enumerator.Current.Value);
        }
      }

      public object Key
      {
        get
        {
          return (object) this.m_enumerator.Current.Key;
        }
      }

      public object Value
      {
        get
        {
          return (object) this.m_enumerator.Current.Value;
        }
      }

      public object Current
      {
        get
        {
          return (object) this.Entry;
        }
      }

      internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> dictionary)
      {
        this.m_enumerator = dictionary.GetEnumerator();
      }

      public bool MoveNext()
      {
        return this.m_enumerator.MoveNext();
      }

      public void Reset()
      {
        this.m_enumerator.Reset();
      }
    }
  }
}
