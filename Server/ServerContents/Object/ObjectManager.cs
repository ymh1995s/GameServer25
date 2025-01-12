using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ServerContents.Object
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        object _lock = new object();

        // 서버와 연결된 모든 오브젝트를 딕셔너리로 관리 (여러 GameRoom의 객체를 총괄)
        // TODO 종류가 여러개면 _objects를 나눌 수 있음
        int _id = 0;
        Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

        // 생성된 오브젝트에 아이디를 부여하고 관리 대상 딕셔너리에 넣음
        // TODO : 오브젝트 다양화되면 제네릭 타입으로 변경
        public GameObject Add()
        {
            GameObject gameObject = new GameObject();

            lock (_lock)
            {
                gameObject.Id = _id++;
                _objects.Add(gameObject.Id, gameObject);
            }

            return gameObject;
        }

        // 관리 대상에서(딕셔너리) 삭제
        public bool Remove(int objectId)
        {
            lock (_lock)
            {
                return _objects.Remove(objectId);
            }
        }

        // 관리 대상에서(딕셔너리) 찾기
        public GameObject Find(int objectId)
        {
            lock (_lock)
            {
                GameObject _object = null;
                if (_objects.TryGetValue(objectId, out _object))
                    return _object;
            }

            return null;
        }
    }
}
