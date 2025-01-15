using Google.Protobuf.Protocol;
using ServerContents.Room;
using ServerContents.Session;

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
        public GameRoom Room { get; set; }
        public StatInfo Stat { get; private set; } = new StatInfo();

        public GameObject()
        {
            PositionInfo dummyPosition =  new PositionInfo();
            dummyPosition.DestinationPosX = 0;
            dummyPosition.DestinationPosY = 0;
            dummyPosition.DestinationPosZ = 0;

            Info.PosInfo = dummyPosition;
            Info.StatInfo = Stat;
        }
    }
}
