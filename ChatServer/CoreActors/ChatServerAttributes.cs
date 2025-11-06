

using Common;
using Server;
using System.Security.AccessControl;

namespace ChatServer.Attribute;


public class IChatServer_Attributes
{
    Actor? owner;
    public int hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;

            MemoryStream stream = new MemoryStream();
            Common.Packer.PackInt(stream, hp);

            Type ownerType = this.owner!.GetType();
            var info = ownerType.GetMethod("on_hp_changed");
            if (info != null)
            {
                info.Invoke(this, null);
            }
        }
    }

    public void SetOwner(Actor owner)
    {
        this.owner = owner;
    }
}