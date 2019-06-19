using System.Configuration;

namespace Lextm.MSBuildLaunchPad.Configuration
{
    [ConfigurationCollection(typeof(ScriptToolAddonElement),
        CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class ScriptToolAddonCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public ScriptToolAddonElement this[int index]
        {
            get { return (ScriptToolAddonElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                base.BaseAdd(index, value);
            }
        }

        public new ScriptToolAddonElement this[string name]
        {
            get
            {
                var exact = (ScriptToolAddonElement)BaseGet(name);
                if (exact == null)
                {
                    foreach (ScriptToolAddonElement item in this)
                    {
                        if (item.Tool == "*")
                        {
                            return item;
                        }
                    }
                }

                return exact;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ScriptToolAddonElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ScriptToolAddonElement)element).Tool;
        }
    }
}
