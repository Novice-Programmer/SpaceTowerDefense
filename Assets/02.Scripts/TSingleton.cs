using UnityEngine;

public class TSingleton<T> : MonoBehaviour where T : TSingleton<T>
{
    [SerializeField] protected bool _dontDestroy = true;
    static volatile T _uniqueInstance = null; // volatile 항상 메모리에 접근
    static volatile GameObject _uniqueObject = null;

    // 프로텍트로 생성자를 제한 해두면 객체 생성을 할 수 없이 상속으로만 가능하다
    protected TSingleton()
    {

    }

    public static T Instance
    {
        get
        {
            if (_uniqueInstance == null)
            {
                // 문제의 발생을 해소하기 위해 메모리를 사용을 못하게 락을 건다.
                lock (typeof(T))
                {
                    if (_uniqueInstance == null && _uniqueObject == null)
                    {
                        _uniqueObject = new GameObject(typeof(T).Name, typeof(T)); // 클래스 명으로 오브젝트를 만들고 클래스를 붙인다.
                        _uniqueInstance = _uniqueObject.GetComponent<T>();
                        _uniqueInstance.Init();
                    }
                }
            }
            return _uniqueInstance;
        }

        protected set
        {
            _uniqueInstance = value;
        }
    }

    protected virtual void Init()
    {
        if (_dontDestroy)
            DontDestroyOnLoad(gameObject);
    }
}
