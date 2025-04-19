# 🔧 Unity 시스템 설계 및 최적화 프로젝트

### 👤 인원  
- 게임 프로그래머 1인 (개인 프로젝트)

---

### 🎯 프로젝트 개요

이 프로젝트는 Unity 개발 과정에서 반복적으로 마주치는 **생명주기 관리, 씬 전환, 오브젝트 관리, 메모리 최적화**와 같은 이슈들을 보다 구조적으로 해결하고자 시작되었습니다.  
**생산성과 유지보수성을 높이는 구조를 목표**로, 실제 개발 환경에서 효율적인 대응이 가능하도록 설계되었습니다.

---

### 🛠 주요 기능 및 설계 구조

#### 🔹 생명주기 관리 시스템
- `Managers.Init` 시스템으로 Core 레이어 초기화를 직접 관리  
- `RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)`을 통해 `Awake` 이전 초기화 보장  
- `DontDestroyOnLoad`으로 필요한 객체의 지속적인 유지  
- `SceneJobLoader`를 통해 씬 로딩 시 초기화 순서를 **우선순위 기반**으로 처리  
- `SortedList<Enum, Action>`을 활용해 직접 순서 지정 가능  
- 싱글톤 패턴으로 동적 객체 접근성 향상

---

#### 🔹 비동기 씬 전환 시스템
- `Async` 방식으로 씬 전환 시 프레임 드롭 최소화  
- `ObjectManager`로 필요한 오브젝트를 미리 메모리에 적재  
- `Addressable`을 활용해 리소스를 유연하게 로딩하고 메모리 사용량 최적화  

  ##### Addressable 도입 전 문제점
  1. 모든 데이터를 빌드 시 포함해야 함  
  2. 객체 수정 시 재빌드 필요  
  3. 런타임 시 전체 리소스를 로드하여 메모리 낭비 발생  

- `Dictionary` 기반 캐싱으로 비동기 로딩 시 오브젝트 누락 방지 및 빠른 반환 지원

---

#### 🔹 ObjectPool 기반 메모리 관리
- `IObjectPoolable` 인터페이스로 객체 재사용 관리  
- `ObjectPool<T>` 구조를 통해 메모리 할당/해제 비용 절감  
- `GetComponent<T>` 호출 최소화를 위한 **제너릭 기반 최적화 구조**

---

### 🚀 트러블슈팅 & 성능 개선 사례

#### 🔸 Addressable 비동기 로딩 충돌 문제 해결
- **문제점:** Addressable handle이 `isDone == false` 상태에서 멈춤  
- **원인:** Addressable 비동기 로딩과 씬 전환 비동기 로직 간 충돌  
- **해결:**  
  1. 초기에는 `Resources` 방식으로 우회  
  2. 디버깅 중 충돌 지점 확인  
  3. Addressable handle이 정상 업데이트되도록 로직 순서 재조정  

---

### ✅ 프로젝트 목표 요약
- 반복적인 Unity 작업을 **자동화/구조화**하여 개발 속도 향상  
- 안정적인 씬 전환 및 **메모리 효율 개선**  
- 협업과 유지보수를 고려한 구조 설계

---

### 📁 기술 스택
- Unity 2021+
- C#
- Addressable
- Object Pool
- Singleton Pattern
- Async / Await
