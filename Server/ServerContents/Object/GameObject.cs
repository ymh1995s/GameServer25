using Google.Protobuf.Protocol;
using ServerContents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerContents.Object
{
    public class GameObject
    {
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        public ObjectInfo Info { get; set; } = new ObjectInfo();

        public ClientSession Session { get; set; }

        // TODO? : 룸 생기면
        // public GameRoom Room { get; set; }

        public GameObject()
        {
            PositionInfo dummyPosition =  new PositionInfo();
            dummyPosition.DestinationPosX = 0;
            dummyPosition.DestinationPosY = 0;
            dummyPosition.DestinationPosZ = 0;

            Info.PosInfo = dummyPosition;
        }
    }
}
