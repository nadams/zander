package util

type Set[K comparable, V any] struct {
	m map[K]V
}

func NewSet[K comparable, V any]() *Set[K, V] {
	return &Set[K, V]{
		m: map[K]V{},
	}
}

func (s *Set[K, V]) Put(key K, value V) {
	s.m[key] = value
}

func (s *Set[K, V]) Has(key K) bool {
	_, found := s.m[key]

	return found
}

func (s *Set[K, V]) Get(key K) V {
	return s.m[key]
}

func (s *Set[K, V]) Del(key K) V {
	t := s.m[key]
	delete(s.m, key)

	return t
}

func (s *Set[K, V]) Keys() []K {
	r := make([]K, 0, len(s.m))

	for k := range s.m {
		r = append(r, k)
	}

	return r
}

func (s *Set[K, V]) Values() []V {
	r := make([]V, 0, len(s.m))

	for _, x := range s.m {
		r = append(r, x)
	}

	return r
}

func (s *Set[K, V]) Entries() map[K]V {
	r := map[K]V{}

	for k, x := range s.m {
		r[k] = x
	}

	return r
}

func (s *Set[K, V]) Intersection(s2 *Set[K, V]) *Set[K, V] {
	out := NewSet[K, V]()

	for k, v := range s.m {
		if s2.Has(k) {
			out.Put(k, v)
		}
	}

	return out
}

func (s *Set[K, V]) Union(s2 *Set[K, V]) *Set[K, V] {
	out := NewSet[K, V]()

	for k, v := range s.m {
		out.Put(k, v)
	}

	for k, v := range s2.m {
		out.Put(k, v)
	}

	return out
}

func (s *Set[K, V]) Difference(s2 *Set[K, V]) *Set[K, V] {
	out := NewSet[K, V]()

	for k, v := range s.m {
		if !s2.Has(k) {
			out.Put(k, v)
		}
	}

	return out
}
