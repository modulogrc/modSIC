using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace OvalDefinitionsGenerator
{
    public class DefinitionsGeneratorConfiguration : ConfigurationSection
    {
        private static readonly DefinitionsGeneratorConfiguration _settings = 
            ConfigurationManager.GetSection("definitionMapping") as DefinitionsGeneratorConfiguration;

        public static DefinitionsGeneratorConfiguration Settings
        {
            get
            {
                return DefinitionsGeneratorConfiguration._settings ?? new DefinitionsGeneratorConfiguration();
            }
        }

        [ConfigurationProperty("definitions", IsDefaultCollection = true, IsRequired = false)]
        [ConfigurationCollection(typeof(DefinitionsCollection), AddItemName = "definition", ClearItemsName = "clear", RemoveItemName = "remove")]
        public DefinitionsCollection Definitions
        {
            get
            {
                return (DefinitionsCollection)this["definitions"];
            }
        }
    }


    [ConfigurationCollection(typeof(DefinitionsCollection))]
    public class DefinitionsCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((CollectConfigurationElement)element).ID;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CollectConfigurationElement();
        }

        public CollectConfigurationElement this[int index]
        {
            get { return (CollectConfigurationElement)BaseGet(index); }

            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public int IndexOf(CollectConfigurationElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(CollectConfigurationElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class CollectConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string ID { get { return (string)this["id"]; } }

        [ConfigurationProperty("tracks", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(TrackCollection), AddItemName = "track", ClearItemsName = "clear", RemoveItemName = "remove")]
        public TrackCollection Tracks { get { return (TrackCollection)this["tracks"]; } }
    }

    [ConfigurationCollection(typeof(TrackCollection))]
    public class TrackCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((TrackElement)element).Id;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TrackElement();
        }

        public TrackElement this[int index]
        {
            get { return (TrackElement)BaseGet(index); }

            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public int IndexOf(TrackElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(TrackElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class TrackElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value.ToString(); }
        }
    }
}
