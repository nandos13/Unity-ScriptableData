using System;
using UnityEngine;

namespace JakePerry.Unity.ScriptableData
{
    /// <summary>
    /// Provides a non-generic base class for a scriptable data container. This type can be used as
    /// the generic argument in a Resources/Addressables LoadAll method, to load all data containers
    /// of any data type in a given directory.
    /// </summary>
    [CreateAssetMenu(fileName = "New Data Container", menuName = "JakePerry/ScriptableData/DataContainer")]
    public class DataContainer : ScriptableObject
    {
        /// <summary>
        /// Indicates the type of data serialized in this container. This property will return
        /// <see langword="null"/> when no data is present.
        /// </summary>
        public virtual Type DataType => null;

        /// <summary>
        /// Gets the data serialized in this container, or <see langword="null"/> if no data is present.
        /// </summary>
        /// <remarks>
        /// Note: This method returns <see cref="object"/> and as such is subject to boxing when invoked.
        /// To retrieve the serialized struct data without incurring unnecessary boxing, cast this container
        /// object to the appropriate <see cref="DataContainer{T}"/> type and access
        /// the <see cref="DataContainer{T}.Data"/> property.
        /// </remarks>
        /// <returns>The serialized data, boxed in an <see cref="object"/>.</returns>
        public virtual object GetData() => null;
    }

    /// <summary>
    /// A generic implementation of the <see cref="DataContainer"/> class.
    /// </summary>
    /// <typeparam name="T">
    /// The type of data to be serialized in this container.
    /// </typeparam>
    public abstract class DataContainer<T> : DataContainer
        where T : struct
    {
        /// <summary>
        /// The serialized backing field for data of type <see cref="T"/>.
        /// </summary>
        [SerializeField]
        private T m_data;

        /// <summary>
        /// Gets the data serialized in this container.
        /// </summary>
        public T Data => m_data;

        /// <summary>
        /// Indicates the type of data serialized in this container.
        /// </summary>
        public override Type DataType => typeof(T);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation

        public override object GetData() => Data;

#pragma warning restore HAA0601
    }
}
