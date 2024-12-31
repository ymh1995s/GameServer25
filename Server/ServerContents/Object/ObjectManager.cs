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
        Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

        int _id = 0;

        // TODO : 오브젝트 다양화되면 제네릭 타입으로 변경
        public GameObject Add()
        {
            GameObject gameObject = new GameObject();

            lock (_lock)
            {
                gameObject.Id = GenerateId();
                _objects.Add(gameObject.Id, gameObject);
            }

            return gameObject;
        }

        int GenerateId()
        {
            lock (_lock)
            {
                return _id++;
            }
        }

        public bool Remove(int objectId)
        {
            lock (_lock)
            {
                return _objects.Remove(objectId);
            }
        }

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
