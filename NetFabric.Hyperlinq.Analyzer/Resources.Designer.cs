﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetFabric.Hyperlinq.Analyzer {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NetFabric.Hyperlinq.Analyzer.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This collection has a value type enumerator. Assigning it to an interface cause it to be boxed and method calls to be virtual, affecting performance..
        /// </summary>
        internal static string AssignmentBoxing_Description {
            get {
                return ResourceManager.GetString("AssignmentBoxing_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; has a value type enumerator. Assigning it to &apos;{1}&apos; causes boxing of the enumerator..
        /// </summary>
        internal static string AssignmentBoxing_MessageFormat {
            get {
                return ResourceManager.GetString("AssignmentBoxing_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assigment to interface causes boxing of enumerator.
        /// </summary>
        internal static string AssignmentBoxing_Title {
            get {
                return ResourceManager.GetString("AssignmentBoxing_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Single() and SingleOrDefault() may perform much more iterations to perform unnecessary validation. Use First() or FirstOrDefault() instead..
        /// </summary>
        internal static string AvoidSingle_Description {
            get {
                return ResourceManager.GetString("AvoidSingle_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Avoid the use of &apos;{0}&apos;. Use &apos;{1}&apos; instead..
        /// </summary>
        internal static string AvoidSingle_MessageFormat {
            get {
                return ResourceManager.GetString("AvoidSingle_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Avoid Single() and SingleOrDefault().
        /// </summary>
        internal static string AvoidSingle_Title {
            get {
                return ResourceManager.GetString("AvoidSingle_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Returning a value-type enumerator allows &apos;foreach&apos; loops to perform better. It will allocate the enumerator on the heap and calls to enumerator methods are not virtual..
        /// </summary>
        internal static string GetEnumeratorReturnType_Description {
            get {
                return ResourceManager.GetString("GetEnumeratorReturnType_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; returns a reference type. Consider returning a value type..
        /// </summary>
        internal static string GetEnumeratorReturnType_MessageFormat {
            get {
                return ResourceManager.GetString("GetEnumeratorReturnType_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GetEnumerator() or GetAsyncEnumerator() should return a value type.
        /// </summary>
        internal static string GetEnumeratorReturnType_Title {
            get {
                return ResourceManager.GetString("GetEnumeratorReturnType_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Public methods should return highest admissible level interface..
        /// </summary>
        internal static string HighestLevelInterface_Description {
            get {
                return ResourceManager.GetString("HighestLevelInterface_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider returning &apos;{0}&apos; instead.
        /// </summary>
        internal static string HighestLevelInterface_MessageFormat {
            get {
                return ResourceManager.GetString("HighestLevelInterface_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Public methods should return highest admissible level interface.
        /// </summary>
        internal static string HighestLevelInterface_Title {
            get {
                return ResourceManager.GetString("HighestLevelInterface_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumerator returned has an empty Dispose(). Consider returning a non-disposable enumerator..
        /// </summary>
        internal static string NonDisposableEnumerator_Description {
            get {
                return ResourceManager.GetString("NonDisposableEnumerator_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; has an empty Dispose(). Consider returning a non-disposable enumerator..
        /// </summary>
        internal static string NonDisposableEnumerator_MessageFormat {
            get {
                return ResourceManager.GetString("NonDisposableEnumerator_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider returning a non-disposable enumerator.
        /// </summary>
        internal static string NonDisposableEnumerator_Title {
            get {
                return ResourceManager.GetString("NonDisposableEnumerator_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumerable can be empty but not null. &apos;foreach&apos; does not check for null..
        /// </summary>
        internal static string NullEnumerable_Description {
            get {
                return ResourceManager.GetString("NullEnumerable_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumerable cannot be null. Return an empty enumerable instead..
        /// </summary>
        internal static string NullEnumerable_MessageFormat {
            get {
                return ResourceManager.GetString("NullEnumerable_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumerable cannot be null.
        /// </summary>
        internal static string NullEnumerable_Title {
            get {
                return ResourceManager.GetString("NullEnumerable_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mutable value-type enumerators cannot be stored in a &apos;readonly&apos; field..
        /// </summary>
        internal static string ReadOnlyEnumeratorField_Description {
            get {
                return ResourceManager.GetString("ReadOnlyEnumeratorField_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is a mutable value-type enumerator. It cannot be stored in a &apos;readonly&apos; field..
        /// </summary>
        internal static string ReadOnlyEnumeratorField_MessageFormat {
            get {
                return ResourceManager.GetString("ReadOnlyEnumeratorField_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mutable value-type enumerators cannot be stored in a &apos;readonly&apos; field.
        /// </summary>
        internal static string ReadOnlyEnumeratorField_Title {
            get {
                return ResourceManager.GetString("ReadOnlyEnumeratorField_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumerable is a value type. If it&apos;s immutable, add the &apos;readonly&apos; modifier..
        /// </summary>
        internal static string ReadOnlyRefEnumerable_Description {
            get {
                return ResourceManager.GetString("ReadOnlyRefEnumerable_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is a value type. Consider making it &apos;readonly&apos;..
        /// </summary>
        internal static string ReadOnlyRefEnumerable_MessageFormat {
            get {
                return ResourceManager.GetString("ReadOnlyRefEnumerable_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumerable is a value type.
        /// </summary>
        internal static string ReadOnlyRefEnumerable_Title {
            get {
                return ResourceManager.GetString("ReadOnlyRefEnumerable_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumerator returns a reference to the item. The type should be declared acordingly so that no copies are performed..
        /// </summary>
        internal static string RefEnumerationVariable_Description {
            get {
                return ResourceManager.GetString("RefEnumerationVariable_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumerator returns a reference to the item. Add &apos;{0}&apos; to the item type..
        /// </summary>
        internal static string RefEnumerationVariable_MessageFormat {
            get {
                return ResourceManager.GetString("RefEnumerationVariable_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The enumerator returns a reference to the item.
        /// </summary>
        internal static string RefEnumerationVariable_Title {
            get {
                return ResourceManager.GetString("RefEnumerationVariable_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The method is empty and is only required when an enumerator interface is implemented..
        /// </summary>
        internal static string RemoveOptionalMethods_Description {
            get {
                return ResourceManager.GetString("RemoveOptionalMethods_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider removing the empty optional enumerator method &apos;{0}&apos;.
        /// </summary>
        internal static string RemoveOptionalMethods_MessageFormat {
            get {
                return ResourceManager.GetString("RemoveOptionalMethods_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider removing an empty optional enumerator method.
        /// </summary>
        internal static string RemoveOptionalMethods_Title {
            get {
                return ResourceManager.GetString("RemoveOptionalMethods_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using CollectionsMarshal.AsSpan() instead of foreach with List&lt;T&gt;..
        /// </summary>
        internal static string UseCollectionsMarshalAsSpan_Description {
            get {
                return ResourceManager.GetString("UseCollectionsMarshalAsSpan_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using CollectionsMarshal.AsSpan() instead of foreach with List&lt;{0}&gt;.
        /// </summary>
        internal static string UseCollectionsMarshalAsSpan_MessageFormat {
            get {
                return ResourceManager.GetString("UseCollectionsMarshalAsSpan_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using CollectionsMarshal.AsSpan().
        /// </summary>
        internal static string UseCollectionsMarshalAsSpan_Title {
            get {
                return ResourceManager.GetString("UseCollectionsMarshalAsSpan_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using &apos;foreach&apos; instead of &apos;for&apos; for iterating over arrays, Spans, or ReadOnlySpans..
        /// </summary>
        internal static string UseForEachLoop_Description {
            get {
                return ResourceManager.GetString("UseForEachLoop_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using &apos;foreach&apos; loop instead of &apos;for&apos; loop for iterating over {0}.
        /// </summary>
        internal static string UseForEachLoop_MessageFormat {
            get {
                return ResourceManager.GetString("UseForEachLoop_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using &apos;foreach&apos; instead of &apos;for&apos; for iterating over arrays or spans.
        /// </summary>
        internal static string UseForEachLoop_Title {
            get {
                return ResourceManager.GetString("UseForEachLoop_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using &apos;for&apos; instead of &apos;foreach&apos; for iterating over collections featuring an indexer.
        /// </summary>
        internal static string UseForLoop_Description {
            get {
                return ResourceManager.GetString("UseForLoop_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using &apos;for&apos; instead of &apos;foreach&apos; for iterating over collections featuring an indexer.
        /// </summary>
        internal static string UseForLoop_MessageFormat {
            get {
                return ResourceManager.GetString("UseForLoop_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using &apos;for&apos; instead of &apos;foreach&apos; for indexer collections.
        /// </summary>
        internal static string UseForLoop_Title {
            get {
                return ResourceManager.GetString("UseForLoop_Title", resourceCulture);
            }
        }
    }
}
