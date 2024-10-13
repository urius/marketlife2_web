using System;
using System.Collections.Generic;
using Data;

namespace Model
{
    public class PlayerDressesModel
    {
        public event Action<ManSpriteType> TopDressTypeChanged;
        public event Action<ManSpriteType> BottomDressTypeChanged;
        public event Action<ManSpriteType> HairTypeChanged;
        public event Action<ManSpriteType> GlassesTypeChanged;
        public event Action<ManSpriteType> TopDressBought;
        public event Action<ManSpriteType> BottomDressBought;
        public event Action<ManSpriteType> HairBought;
        public event Action<ManSpriteType> GlassesBought;

        private ManSpriteType _topDressType;
        private ManSpriteType _bottomDressType;
        private ManSpriteType _hairType;
        private ManSpriteType _glassesType;
        
        private readonly List<ManSpriteType> _boughtTopDresses;
        private readonly List<ManSpriteType> _boughtBottomDresses;
        private readonly List<ManSpriteType> _boughtHairs;
        private readonly List<ManSpriteType> _boughtGlasses;

        public PlayerDressesModel(
            ManSpriteType topDressType,
            ManSpriteType bottomDressType,
            ManSpriteType hairType,
            ManSpriteType glassesType,
            ManSpriteType[] boughtTopDresses,
            ManSpriteType[] boughtBottomDresses,
            ManSpriteType[] boughtHairs,
            ManSpriteType[] boughtGlasses)
        {
            _topDressType = topDressType;
            _bottomDressType = bottomDressType;
            _hairType = hairType;
            _glassesType = glassesType;

            _boughtTopDresses = new List<ManSpriteType>(boughtTopDresses);
            AddBoughtTopDress(_topDressType);

            _boughtBottomDresses = new List<ManSpriteType>(boughtBottomDresses);
            AddBoughtBottomDress(_bottomDressType);

            _boughtHairs = new List<ManSpriteType>(boughtHairs);
            AddBoughtHair(_hairType);

            _boughtGlasses = new List<ManSpriteType>(boughtGlasses);
            AddBoughtGlasses(_glassesType);
        }
        
        public IReadOnlyList<ManSpriteType> BoughtTopDresses => _boughtTopDresses.AsReadOnly();
        public IReadOnlyList<ManSpriteType> BoughtBottomDresses => _boughtBottomDresses.AsReadOnly();
        public IReadOnlyList<ManSpriteType> BoughtHairs => _boughtHairs.AsReadOnly();
        public IReadOnlyList<ManSpriteType> BoughtGlasses => _boughtGlasses.AsReadOnly();

        public ManSpriteType TopDressType
        {
            get => _topDressType;
            set
            {
                _topDressType = value;
                TopDressTypeChanged?.Invoke(value);
            }
        }

        public ManSpriteType BottomDressType
        {
            get => _bottomDressType;
            set
            {
                _bottomDressType = value;
                BottomDressTypeChanged?.Invoke(value);
            }
        }

        public ManSpriteType HairType
        {
            get => _hairType;
            set
            {
                _hairType = value;
                HairTypeChanged?.Invoke(value);
            }
        }

        public ManSpriteType GlassesType
        {
            get => _glassesType;
            set
            {
                _glassesType = value;
                GlassesTypeChanged?.Invoke(value);
            }
        }

        public void AddBoughtTopDress(ManSpriteType topDress)
        {
            if (_boughtTopDresses.Contains(topDress)) return;
            
            _boughtTopDresses.Add(topDress);
            
            TopDressBought?.Invoke(topDress);
        }

        public void AddBoughtBottomDress(ManSpriteType topDress)
        {
            if (_boughtBottomDresses.Contains(topDress)) return;

            _boughtBottomDresses.Add(topDress);

            BottomDressBought?.Invoke(topDress);
        }

        public void AddBoughtHair(ManSpriteType hair)
        {
            if (_boughtHairs.Contains(hair)) return;
    
            _boughtHairs.Add(hair);
    
            HairBought?.Invoke(hair);
        }

        public void AddBoughtGlasses(ManSpriteType glasses)
        {
            if (_boughtGlasses.Contains(glasses)) return;
    
            _boughtGlasses.Add(glasses);
    
            GlassesBought?.Invoke(glasses);
        }
    }
}