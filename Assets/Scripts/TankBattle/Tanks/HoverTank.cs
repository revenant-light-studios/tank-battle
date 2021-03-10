using ExtensionMethods;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

namespace TankBattle.Tanks
{
    public class HoverTank : MonoBehaviour
    {
        public bool ExtraFuelTank;
        public bool RocketLauncher;
        public float FlightDistance = 0.6f;
        public float HoverForce = 80000;
        private float _deadZone = 0.1f;
        public float ForwardAccel = 10000.0f;
        public float BackwardAccel = 2500.0f;

        private float _thrust = 0;
        public float TurnRate = 500f;
        private float _turn;

        RaycastHit _raycastHit;
        
        private Rigidbody _rigidbody;
        private GameObject _tankBase;
        private Turret _turret;
        private GameObject _extraFuelTank;
        private GameObject _rocketLauncher;

        private readonly GameObject[] _antyGravityEngines = new GameObject[4];
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _tankBase = transform.FirstOrDefault(t => t.name == "Body").gameObject;
            _turret = transform.FirstOrDefault(t => t.name == "Turret").GetComponent<Turret>();
            _extraFuelTank = transform.FirstOrDefault(t => t.name == "ExtraFuelTank").gameObject;
            _rocketLauncher = transform.FirstOrDefault(t => t.name == "MissileThrower").gameObject;
        }

        private void Start()
        {
            _extraFuelTank.SetActive(ExtraFuelTank);
            _rocketLauncher.SetActive(RocketLauncher);
            _rigidbody.centerOfMass = Vector3.down;
            
            UpdateAntiGravityEngines();
            
            // TODO: Move this out of here
            Cursor.visible = false;
        }

        private void UpdateAntiGravityEngines()
        {
            float tankWidth = _tankBase.GetComponent<MeshFilter>().mesh.bounds.size.x;
            float tankLength = _tankBase.GetComponent<MeshFilter>().mesh.bounds.size.z;

            _antyGravityEngines[0] = new GameObject("Engine0");
            _antyGravityEngines[0].transform.parent = this.transform; 
            _antyGravityEngines[0].transform.position = new Vector3(transform.position.x - tankWidth * 0.5f, transform.position.y, transform.position.z - tankLength * 0.5f);

            _antyGravityEngines[1] = new GameObject("Engine1");
            _antyGravityEngines[1].transform.parent = this.transform;
            _antyGravityEngines[1].transform.position = new Vector3(transform.position.x + tankWidth * 0.5f, transform.position.y, transform.position.z - tankLength * 0.5f);
            
            _antyGravityEngines[2] = new GameObject("Engine2");
            _antyGravityEngines[2].transform.parent = this.transform;
            _antyGravityEngines[2].transform.position = new Vector3(transform.position.x - tankWidth * 0.5f, transform.position.y, transform.position.z + tankLength * 0.5f);
            
            _antyGravityEngines[3] = new GameObject("Engine3");
            _antyGravityEngines[3].transform.parent = this.transform;
            _antyGravityEngines[3].transform.position = new Vector3(transform.position.x + tankWidth * 0.5f, transform.position.y, transform.position.z + tankLength * 0.5f);
        }
        
        private void Update()
        {
            // Thrusting
            _thrust = 0f;
            float forward = Input.GetAxis("Vertical");
            if (forward > _deadZone)
            {
                _thrust = forward * ForwardAccel;
            }
            else if(forward < -_deadZone)
            {
                _thrust = forward * BackwardAccel;
            }

            // Turning
            _turn = 0f;
            float turn = Input.GetAxis("Horizontal");
            if (Mathf.Abs(turn) > _deadZone)
            {
                _turn = turn;
            }
        }
        
        private void FixedUpdate()
        {
            if (Mathf.Abs(_thrust) > 0f)
            {
                _rigidbody.AddForce(transform.forward * _thrust);
            }

            if (Mathf.Abs(_turn) > 0f)
            {
                _rigidbody.AddTorque(Vector3.up * (_turn * TurnRate));
            }

            // Hovering
            for(int i=0; i < _antyGravityEngines.Length; i++)
            {
                GameObject engine = _antyGravityEngines[i];
                
                if (Physics.Raycast(engine.transform.position, -transform.up, out _raycastHit, FlightDistance))
                {
                    _rigidbody.AddForceAtPosition(Vector3.up 
                        * (HoverForce
                        * (1.0f - (_raycastHit.distance / FlightDistance))),
                        engine.transform.position);
                }
                else
                {
                    if (transform.position.y > engine.transform.position.y)
                    {
                        _rigidbody.AddForceAtPosition(Vector3.up * HoverForce,
                            engine.transform.position);
                    }
                    else
                    {
                        _rigidbody.AddForceAtPosition(Vector3.up * -HoverForce,
                            engine.transform.position);
                    }
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if(_antyGravityEngines[0] != null) Gizmos.DrawSphere(_antyGravityEngines[0].transform.position, 0.2f);
            if(_antyGravityEngines[1] != null) Gizmos.DrawSphere(_antyGravityEngines[1].transform.position, 0.2f);
            if(_antyGravityEngines[2] != null) Gizmos.DrawSphere(_antyGravityEngines[2].transform.position, 0.2f);
            if(_antyGravityEngines[3] != null) Gizmos.DrawSphere(_antyGravityEngines[3].transform.position, 0.21f);
        }
    }
}