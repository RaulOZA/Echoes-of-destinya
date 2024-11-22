using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metroidvania.Combat; // Agregar el espacio de nombres para usar CharacterHitData

namespace Metroidvania
{
    public class BlackRayController : MonoBehaviour
    {
        private int _direction;
        private float _speed;
        private int _damage;
        private bool _isActive; // Controla si el poder está activo

        public void Initialize(int direction, float speed, int damage)
        {
            _direction = direction;
            _speed = speed;
            _damage = damage;
            _isActive = false; // Inicia como desactivado
            gameObject.SetActive(false); // Desactiva el objeto al inicio
        }

        private void Update()
        {
            if (_isActive)
            {
                // Mover el rayo
                transform.Translate(Vector2.right * _direction * _speed * Time.deltaTime);
            }
        }

        public void ActivatePower()
        {
            _isActive = true;
            gameObject.SetActive(true); // Activa el objeto
            GetComponent<Animator>().SetTrigger(BlackRayAnimHash); // Activa la animación
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isActive && other.TryGetComponent<IHittableTarget>(out IHittableTarget hittableTarget))
            {
                CharacterHitData hitData = new CharacterHitData(_damage, 0, null);
                hittableTarget.OnTakeHit(hitData);
                Destroy(gameObject); // Destruir el rayo después de impactar
            }
        }

        public void DeactivatePower()
        {
            _isActive = false;
            gameObject.SetActive(false); // Desactiva el objeto
        }

        public static readonly int BlackRayAnimHash = Animator.StringToHash("BlackRayAttack");
    }
}
