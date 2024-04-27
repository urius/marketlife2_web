using UnityEngine;

namespace Other
{
    public class LabeledArrayAttribute : PropertyAttribute
    {
        public readonly string FieldName;
    
        public LabeledArrayAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}