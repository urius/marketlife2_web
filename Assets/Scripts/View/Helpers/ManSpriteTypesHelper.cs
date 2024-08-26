using System;
using Data;
using Random = UnityEngine.Random;

namespace View.Helpers
{
    public abstract class ManSpriteTypesHelper
    {
        private static readonly int MaxBodyClothesInt = GetMaxClothesInt(ManSpriteType.Clothes1);
        private static readonly int MaxHairInt = GetMaxClothesInt(ManSpriteType.Hair1);
        private static readonly int MaxGlassesInt = GetMaxClothesInt(ManSpriteType.Glasses1);
        private static readonly int HandBodyClothesDelta = ManSpriteType.HandClothes1 - ManSpriteType.Clothes1;
        private static readonly int FootBodyClothesDelta = ManSpriteType.FootClothes1 - ManSpriteType.Clothes1;
        private static readonly ManSpriteType[] PersonalClothes = { ManSpriteType.Clothes6, ManSpriteType.Clothes7 };
        
        public static (ManSpriteType BodyClothes, ManSpriteType HandClothes, ManSpriteType FootClothes)
            GetCustomerRandomClothes()
        {
            var result = (int)ManSpriteType.Clothes1;
            for (var i = 0; i < 10; i++)
            {
                result = Random.Range((int)ManSpriteType.Clothes1, MaxBodyClothesInt + 1);
                if (Array.IndexOf(PersonalClothes, result) < 0)
                {
                    break;
                }
            }

            var footClothes = Random.Range((int)ManSpriteType.Clothes1, MaxBodyClothesInt + 1) + FootBodyClothesDelta;

            return (
                (ManSpriteType)result,
                (ManSpriteType)(result + HandBodyClothesDelta),
                (ManSpriteType)footClothes);
        }
        public static (ManSpriteType BodyClothes, ManSpriteType HandClothes, ManSpriteType FootClothes)
            GetTruckPointStaffClothes()
        {
            return (ManSpriteType.Clothes7, ManSpriteType.HandClothes7, ManSpriteType.FootClothes7);
        }
        
        public static (ManSpriteType BodyClothes, ManSpriteType HandClothes, ManSpriteType FootClothes)
            GetCashDeskStaffClothes()
        {
            return (ManSpriteType.Clothes6, ManSpriteType.HandClothes6, ManSpriteType.FootClothes6);
        }

        public static (ManSpriteType BodyClothes, ManSpriteType HandClothes, ManSpriteType FootClothes)
            GetClothesByIntType(int clothesIntType)
        {
            return ((ManSpriteType)clothesIntType,
                (ManSpriteType)(clothesIntType + HandBodyClothesDelta),
                (ManSpriteType)clothesIntType + FootBodyClothesDelta);
        }

        public static ManSpriteType GetRandomHair()
        {
            var resultInt = Random.Range((int)ManSpriteType.Hair1, MaxHairInt + 1);

            return (ManSpriteType)resultInt;
        }

        public static ManSpriteType GetRandomGlasses()
        {
            var resultInt = Random.Range((int)ManSpriteType.Glasses1, MaxGlassesInt + 1);

            return (ManSpriteType)resultInt;
        }  

        private static int GetMaxClothesInt(ManSpriteType refSpriteType)
        {
            var result = (int)refSpriteType;
            
            var manSpriteTypes = Enum.GetValues(typeof(ManSpriteType));
            foreach (ManSpriteType type in manSpriteTypes)
            {
                var intType = (int)type;
                if (intType <= result) continue;
                
                if (intType - result == 1)
                {
                    result = (int)type;
                }
                else if(intType - result > 1)
                {
                    break;
                }
            }

            return result;
        }
    }
}