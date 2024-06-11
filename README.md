# SolarSystem
 Solar System with Unity

# Solar System

This repository computationally simulates the solar system.

# Computational science
Computational science is a field that uses numerical methods and computer science to understand or solve complex scientific or engineering problems. Unlike traditional science and engineering methods that rely on theory and experiments to understand subjects, computational science primarily uses computers to interpret mathematical models. [https://en.wikipedia.org/wiki/Computational_science]

Computational science can be applied to various fields, including astronomy. For instance, if there is one central star and one celestial body revolving around it, there are two objects in the system, and it is possible to determine all information about them using mathematical methods. However, once the number of objects exceeds three, solving the problem mathematically becomes extremely difficult and is nearly impossible in most cases. This is known as the three-body problem, where three celestial bodies interact with each other.

However, using computational science, the three-body problem can be solved. Here, solving means finding an approximate solution within an acceptable margin of error, not obtaining a mathematically exact solution. The forces acting on different celestial bodies are gravitational forces, and using these forces, we can calculate acceleration according to Newton's second law ($F=ma$). Integrating acceleration gives velocity, and integrating velocity gives position. This process is **discretized** in a computational environment, which inevitably introduces some error, but the results are reliable.

# Solar System
The solar system contains many celestial bodies, including the Sun, eight planets orbiting it, numerous moons revolving around each planet, and various other celestial objects. Simulating all of these requires substantial computational power. Thus, this repository simulates the Sun, eight planets, and the Moon, resulting in a ten-body problem.

Below is a brief introduction to the solar system simulation process.

# Displacement, Velocity, Acceleration
It is well known that the rate of change of displacement over time is velocity, and the rate of change of velocity over time is acceleration. That is, differentiating displacement gives velocity, and differentiating velocity gives acceleration. Conversely, integrating acceleration gives velocity, and integrating velocity gives displacement. Forces relate to acceleration, and by calculating acceleration through forces and integrating twice, we can determine displacement, indicating where an object is currently located.

Consider a simple case where acceleration $a$ is $a(t) = t$. With initial velocity and position being 0, we first need to calculate the object's velocity:
$$v(t)=\int a(t)dt=\int tdt=\frac{1}{2}t^2+C=\frac{1}{2}t^2$$
Thus, we find the velocity, and by integrating the velocity again, we can find the displacement:
$$x(t)=\int v(t)=\int \frac{1}{2}{t}^2=\frac{1}{6}t^3+C=\frac{1}{6}t^3$$
This is a straightforward calculation, and there is no issue finding the solution analytically.

However, in computational science, integration is used somewhat differently. Although we still find displacement by integrating, we often cannot represent acceleration or velocity as functions, such as $a(t)=t$. Thus, integration is often used in the context of extrapolation. Recall that integration is computed as a sum of certain values; thus, integrating acceleration to compute velocity can be expressed as:
$$v_{t+1}=v_t+a_t \Delta t$$
This is a discrete sum, while true analytic integration is continuous, so there is a difference, and errors can occur. Similarly, displacement can be calculated as:
$$x_{t+1}=x_t+v_t \Delta t$$

These concepts are implemented in code as follows:
(acceleration -> velocity) ``` NowVelocity += acceleration * dt;```
```C#
    public void UpdateVelocity(Body[] bodies, float dt)
    {
        for(int i = 0; i < bodies.Length; i++) { 
            //...
            Vector3 acceleration = force / Mass;
            NowVelocity += acceleration * dt;
        }
    }
```
(velocity -> displacement) ```ThisRigidbody.position += NowVelocity * dt;```
```C#
    public void UpdatePosition(float dt)
    {
        ThisRigidbody.position += NowVelocity * dt;
    }
```
With this method, if we can calculate the force acting on an object, we can determine its displacement.

# Gravitational Force
To find acceleration, we need to calculate the force. As mentioned before, acceleration is related to force via mass. The gravitational force between two objects with masses $M_1$ and $M_2$ is given by:
$$F=G \frac{M_1M_2}{r^2}$$
where $G$ is the gravitational constant, and $r$ is the distance between the two objects.
Celestial bodies, including planets, have mass and interact with other bodies through gravitational forces. While there may be other interactions, they are often negligible compared to gravitational forces, making it sufficient to calculate only gravitational forces for simple celestial movement simulations.

This is implemented in Body.cs as follows:
```Vector3 force = forceDirection * Universe.GravitationalConstant * Mass * other.Mass / sqrDistance;```
```C#
    public void UpdateVelocity(VirtualBody[] bodies, float dt)
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            VirtualBody other = bodies[i];
            if (other == this)
                continue;
            float sqrDistance = (other.Position - Position).sqrMagnitude;
            Vector3 forceDirection = (other.Position - Position).normalized;
            if (sqrDistance < float.Epsilon)
                continue;
            Vector3 force = forceDirection * Universe.GravitationalConstant * Mass * other.Mass / sqrDistance;

            Vector3 acceleration = force / Mass;
            NowVelocity += acceleration * dt;
        }
    }
```
# Solar System Simulation?
Simulating the solar system involves setting appropriate initial conditions for celestial bodies, calculating their interactions through gravitational forces, and computing their displacements by determining their accelerations. Here are the results:
![](https://github.com/Nov10/SolarSystem/assets/91333241/3bc16e49-1fdb-4756-8f5b-d9a37f8f87a0)

https://github.com/Nov10/SolarSystem/assets/91333241/375a0352-b1a2-480b-9030-f612cecea624

With the Sun, eight planets, and the Moon set as ten celestial bodies and given appropriate initial conditions, we can produce the following results.

https://github.com/Nov10/SolarSystem/assets/91333241/b65dcfd4-fde7-4edc-97ef-48fef9fb8834

This shows the Earth and the Moon.

You might wonder: **Aren't the eccentricities of planetary orbits considered? What about Kepler's laws?** No, they are not explicitly considered. Rather, they are automatically accounted for.
If the initial velocities are set correctly to form elliptical orbits, the eccentricity is automatically calculated, and Kepler's laws are naturally implemented as celestial bodies move according to gravitational laws. Thus, simply implementing gravitational forces is sufficient to create a solar system model.


# Kepler's Laws
Let’s confirm if Kepler's laws are indeed implemented. Kepler's laws of planetary motion consist of three laws:
1. Planets move in elliptical orbits with the Sun at one focus.
2. A line segment joining a planet and the Sun sweeps out equal areas during equal intervals of time.
3. The square of a planet's orbital period is proportional to the cube of the semi-major axis of its orbit.

Let’s examine the first and second laws briefly.
#### First Law
The first law states that planets move in elliptical orbits. With proper initial velocities, we can observe such results:

https://github.com/Nov10/SolarSystem/assets/91333241/814a0af4-5995-41c5-8138-7c2ba31e0fec

Planets orbit elliptically.
#### Second Law
The second law states that the area swept out by a line segment joining a planet and the Sun is constant over time. The hatched area represents this concept:

![](https://github.com/Nov10/SolarSystem/assets/91333241/e40fe88e-3f48-4034-b428-d1a199b9d378)

If we graph this area over time, it appears as follows:

![](https://github.com/Nov10/SolarSystem/assets/91333241/4dac8658-5cb4-4955-9c2d-28bd3391a900)

The graph shows a constant value.
Also, Kepler's second law implies that a planet moves faster when it is closer to the Sun and slower when it is farther away. This is also evident from the above video.

https://github.com/Nov10/SolarSystem/assets/91333241/814a0af4-5995-41c5-8138-7c2ba31e0fec

Thus, Kepler's second law is confirmed.
Again, note that Kepler's laws are not explicitly implemented but naturally emerge.

# Lagrange Points
Lagrange points are positions in space where a small celestial body can maintain its position relative to two larger celestial bodies due to the gravitational forces. These points are useful for calculating positions for artificial satellites or rocket trajectories.

We will calculate one of the five Lagrange points (L1) using approximation. For two celestial bodies and a third negligible-mass object, the Lagrange point lies on the line connecting the two large bodies. This point is where the gravitational forces and centrifugal force balance. Thus,

$$\frac{G M_1}{(R - r)^2} - \frac{G M_2}{r^2} = \frac{G (M_1 + M_2)}{R^3} \cdot r$$
Reorganizing,
$$\frac{M_1}{(R-r)^2}-\frac{M_2}{r^2}=(\frac{M_1}{M_1+M_2}R-r)\frac{M_1+M_2}{R^3}$$
If we let $x = \frac{r}{R}$ and $\mu = \frac{M_2}{M_1 + M_2}$, the equation becomes a polynomial in $x$:

$$x^5 + (\mu - 3)x^4 + (3 - 2\mu)x^3 - \mu x^2 + 2\mu x - \mu = 0$$
If the mass difference between the two celestial bodies is significant, we can approximate $\mu=0$, and find the approximate solution:
$$r \approx R \sqrt[3]{\frac{\mu}{3}}$$

This formula allows us to calculate one Lagrange point: ```float L1 = r * (1 - Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));```
```C#
    private Vector3 CalculateLagrangePointL1(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        float mu = body2.Mass / (body1.Mass + body2.Mass);
        float L1 = r * (1 - Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position + direction * L1;
    }
```

We can also approximate the L2 and L3 points as follows:

```C#
    private Vector3 CalculateLagrangePointL2(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        float mu = body2.Mass / (body1.Mass + body2.Mass);
        float L2 = r * (1 + Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position + direction * L2;
    }

    private Vector3 CalculateLagrangePointL3(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position - direction * r;
    }
   ```
Using these methods, we can calculate three Lagrange points.
![Lag1](https://github.com/Nov10/SolarSystem/assets/91333241/e300eb86-466b-4ba4-b016-efaf12d7978d)
![lag2](https://github.com/Nov10/SolarSystem/assets/91333241/91411573-1df0-4d28-be41-7e5dfd5bf620)

https://github.com/Nov10/SolarSystem/assets/91333241/6bc96727-a34a-4e6e-8880-82351a1c0409

The three points in red, green, and blue represent the Lagrange points for the Earth-Moon system.

Calculating the Lagrange points for other planetary systems produces the following results:
![Lag3](https://github.com/Nov10/SolarSystem/assets/91333241/7a521b7a-bc46-4bd1-80ec-0f2272117987)

# Lagrange Points - Three-body Problem?
Calculating Lagrange points for two celestial bodies is relatively straightforward, but the problem becomes significantly more complex when more than three celestial bodies are involved. Even with approximations, calculating the positions mathematically is difficult.

Therefore, we need a different approach, which is one of the core aspects of computational science: **numerical analysis**. **Numerical analysis** involves finding approximate solutions to mathematical problems using numerical techniques. Simply put, it's solving math problems with computers. With numerical analysis, we can solve equations, including those representing Lagrange points. This method is independent of the number of celestial bodies, allowing us to calculate Lagrange points even for systems with 100 bodies if computational power permits.

Ultimately, we need to solve equations, and we will use Newton's method, a well-known numerical technique, to approximate the solutions. Newton's method can be expressed as:
$$x_{n+1}=x_n-\frac{f(x_n)}{f'(x_n)}$$
Although the details are omitted, Newton's method approximates the root of an equation by assuming the root lies to the right or left of the current point ($x_n$) based on the sign of the tangent slope and function value. Repeating this process sufficiently, $x_n$ becomes the Lagrange point for three or more celestial bodies. This can be implemented as follows, where  ```force / forceDerivative.magnitude``` and ```position -= delta;``` are relevant.
```C#    
    private Vector3 CalculateLagrangePoint(Body body1, Body body2, Body body3)
    {
        Vector3 initialGuess = (body1.transform.position + body2.transform.position + body3.transform.position) / 3;
        Vector3 position = initialGuess;
        float tolerance = 1e-4f;
        int maxIterations = 1000;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Vector3 force = Vector3.zero;
            Vector3 forceDerivative = Vector3.zero;

            force += CalculateGravitationalForce(body1, position);
            force += CalculateGravitationalForce(body2, position);
            force += CalculateGravitationalForce(body3, position);

            forceDerivative += CalculateGravitationalForceDerivative(body1, position);
            forceDerivative += CalculateGravitationalForceDerivative(body2, position);
            forceDerivative += CalculateGravitationalForceDerivative(body3, position);

            Vector3 delta = force / forceDerivative.magnitude / 500f;

            position -= delta;

            if (delta.magnitude < tolerance)
            {
                break;
            }
        }

        return position;
    }
```

Running this code gives the following results for the Lagrange points among the Sun, Earth, and Moon.

![L3_z_7_3](https://github.com/Nov10/SolarSystem/assets/91333241/e732fe66-9940-4714-87e2-d0d1d07e9650)

https://github.com/Nov10/SolarSystem/assets/91333241/66d1db7d-51b8-47fb-8b32-86dbf7efef0e

https://github.com/Nov10/SolarSystem/assets/91333241/953173be-b2ee-4144-acee-9248c222c6e7

The points keep moving because the solution is unstable as the celestial bodies continue to move. The Lagrange points change with the current positions of the celestial bodies, and the Newton method's initial starting point is always the center of three celestial bodies, causing this instability. Although the exact position is uncertain due to the lack of much known about three-body Lagrange points, the results are similar to those for two celestial bodies.

# Scale of Values
Finally, we need to discuss the scale of values. Computers are finite calculating machines and can store values only within certain ranges. Therefore, very large numbers are difficult to represent, and we must scale the values to fit within an appropriate range. This repository uses the following scales:

### Distance (length): Distance is calculated by multiplying astronomical units by 300
### Mass : Mass is divided by $10^{23}$

Scaling the values as above brings the mass within a manageable range, but even with such a small scaling factor for distance, the distance from the Sun to Neptune is about 90,000 units. This can cause floating-point errors, visible visually, although it doesn't significantly affect orbit calculations. Therefore, the scale was adjusted to implement the simulation as described above.

# 태양계

이 리포지토리는 태양계를 전산적으로 시뮬레이션합니다.

# 계산과학
계산과학이란, 수치적 방법과 컴퓨터과학을 이용하여 복잡한 과학이나 공학 문제를 이해하거나 해결하는 분야입니다. 계산과학은 기존의 과학과 공학 분야에서 사용되는 방법인 이론 및 실험을 통하여 연구 대상에 대한 이해를 얻어내는 것이 아니라, 주로 컴퓨터를 이용하여 수학적인 모델을 해석하는 방법을 통하여 연구 대상을 이해하는 것을 목적으로 합니다. [https://ko.wikipedia.org/wiki/%EA%B3%84%EC%82%B0%EA%B3%BC%ED%95%99]

계산과학은 다양한 분야에 적용될 수 있으며, 천문학을 비롯한 천체에도 적용될 수 있습니다. 하나의 중심별이 있고, 이 중심별을 기준으로 하여 회전하는 하나의 천체가 있다고 하면, 계에 존재하는 물체는 2개이며, 이는 충분히 수학적 방법을 이용하여 물체에 대한 모든 정보를 알아낼 수 있습니다.
그러나 물체가 3개가 넘어가는 순간 이 문제는 수학적으로 풀기 굉장히 어려워지는데, 특수한 경우가 아니고서는 거의 푸는 것이 불가능하다고 알려져 있습니다. 이 문제를 3개의 천체가 상호작용한다고 하여 3체 문제라고도 합니다.

그러나, 계산과학을 이용하면 3체 문제를 **풀** 수 있습니다. 여기서 '**푼다**'라고 하는 것은, 수학적 엄밀해를 구하는 것이 아닌 적당한 오차 범위 내의 **근사해**를 구하는 것입니다. 서로 다른 천체에 작용하는 힘은 (후술할) 만유인력이며, 이 힘을 통해 뉴턴의 제2법칙($F=ma$)에 따라 가속도를 계산할 수 있으며, 가속도를 적분하면 속도를, 속도를 적분하면 위치를 계산할 수 있습니다. 이 과정은 컴퓨터에서, 즉 전산적인 환경에서 **이산화(discritized)**되어 계산되기 때문에 오차가 필연적으로 발생하나, 그 결과값은 충분히 신뢰할 수준의 품질을 가집니다.

# 태양계
태양계에는 정말 많은 천체가 존재합니다. 태양과 태양을 중심으로 공전하는 8개의 행성, 그리고 그 각각을 중심으로 회전하는 수많은 위성과 여러 천체들이 존재합니다. 이들을 전부 계산하는 것은 계산 성능이 뒷받침되어야 하기에, 이 리포지토리는 간단하게 태양과 8개의 행성, 달을 시뮬레이션합니다. 사실, 이것만 해도 벌써 10체 문제의 영역입니다.

아래는 태양계를 시뮬레이션하는 과정을 간략하게 소개합니다.

# 변위, 속도, 가속도
변위의 시간 변화율은 속도, 속도의 시간 변화율은 가속도라는 사실은 널리 알려져 있습니다. 즉, 변위를 미분하면 속도이며, 속도를 미분하면 가속도입니다. 이를 반대로 말하면, 가속도를 적분하면 속도이며, 속도를 적분하면 변위가 됩니다. 힘은 가속도와 관련이 있으며, 힘을 통해 가속도를 구하고 이를 2번 적분하면 변위, 즉 현재 물체가 어디에 위치하는지 알 수 있습니다.

간단하게 가속도 $a$가 $a(t) = t$인 경우를 생각해 보겠습니다. 초기 속도와 위치는 0입니다. 현재 이 물체의 위치를 구하기 위해선 먼저 물체의 속도를 계산해야 합니다. 속도는 가속도의 적분, 즉 다음과 같이 나타낼 수 있습니다.
$$v(t)=\int a(t)dt=\int tdt=\frac{1}{2}t^2+C=\frac{1}{2}t^2$$
이렇게 속도를 구할 수 있으며, 속도를 다시 한번 적분하면 변위를  구할 수 있습니다. 즉,
$$x(t)=\int v(t)=\int \frac{1}{2}{t}^2=\frac{1}{6}t^3+C=\frac{1}{6}t^3$$
입니다. 꽤나 간단한 계산이며, 해석학적으로 해를 구하는 데에 문제가 없습니다.
그러나, 계산과학에서 적분은 조금 다른 방식으로 사용됩니다. 적분을 통해 변위를 구하는 것은 동일하나, 물체의 가속도 또는 속도를 어떠한 함수로 나타낼 수 없기 때문에, 즉 다시 말해 $a(t)=t$라고 표기할 수가 없기 때문에, 적분은 **외삽(Extrapolation)**의 의미를 가지는 계산으로 사용되는 경우가 많습니다.
적분이 어떠한 값들의 합으로 계산됨을 상기하면, 가속도를 적분하여 속도를 계산하는 식은 다음과 같이 나타낼 수 있습니다.
$$v_{t+1}=v_t+a_t \Delta t$$
물론 위의 식은 이산합이고, 실제 해석학적 적분은 연속합이기에 둘 사이의 차이가 존재하며, 오차가 발생할 수 있습니다.
이와 마찬가지로, 변위는 다음과 같이 계산할 수 있습니다.
$$x_{t+1}=x_t+v_t \Delta t$$

이들은 다음과 같이 코드로 구현되어 있습니다.
(가속도 ->  속도) ``` NowVelocity += acceleration * dt;```
```C#
    public void UpdateVelocity(Body[] bodies, float dt)
    {
        for(int i = 0; i < bodies.Length; i++) { 
            //생략...
            Vector3 acceleration = force / Mass;
            NowVelocity += acceleration * dt;
        }
    }
```
(속도 -> 변위) ```ThisRigidbody.position += NowVelocity * dt;```
```C#
    public void UpdatePosition(float dt)
    {
        ThisRigidbody.position += NowVelocity * dt;
    }
```
위와 같은 방법으로, 물체에 가해지는 힘을 계산할 수 있다면 그 물체의 변위를 계산할 수 있습니다.

# 만유인력
그렇다면 가속도를 구해야 합니다. 이전에 언급했듯, 가속도에 질량을 곱하면 힘이 되는 관계를 통해 가속도를 계산할 수 있습니다.
질량이 $M_1$, $M_2$인 물체 사이에 작용하는 인력, 즉 만유인력은 다음과 같이 나타낼 수 있음이 알려져 있습니다.
$$F=G \frac{M_1M_2}{r^2}$$
$G$는 중력상수이며, $r$은 두 물체 사이의 거리를 나타냅니다.
행성을 비롯한 천체는 질량을 가지고 있으며, 이에 따라 다른 천체와 힘을 주고받는 상호작용을 하게 됩니다. 이들 사이에는 여러 상호작용이 있을 수 있으나, 대부분은 만유인력에 비해 무시할 수 있을 만큼 그 정도가 작아, 천체들의 움직임을 간단하게 계산할 때에는 만유인력만을 계산해도 충분한 결과를 얻을 수 있습니다.

이는 다음과 같이 Body.cs에 구현되어 있습니다. ```Vector3 force = forceDirection * Universe.GravitationalConstant * Mass * other.Mass / sqrDistance;```가 이에 해당합니다.
```C#
    public void UpdateVelocity(VirtualBody[] bodies, float dt)
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            VirtualBody other = bodies[i];
            if (other == this)
                continue;
            float sqrDistance = (other.Position - Position).sqrMagnitude;
            Vector3 forceDirection = (other.Position - Position).normalized;
            if (sqrDistance < float.Epsilon)
                continue;
            Vector3 force = forceDirection * Universe.GravitationalConstant * Mass * other.Mass / sqrDistance;

            Vector3 acceleration = force / Mass;
            NowVelocity += acceleration * dt;
        }
    }
```
# 태양계 시뮬레이션?
태양계 시뮬레이션은 사실 이게 전부 다입니다. 천체들에게 적절한 초기조건을 설정하고, 만유인력으로 상호작용하며, 이 천체들의 가속도를 구해 변위를 계산하면 태양계를 만들어낼 수 있습니다. 결과는 다음과 같습니다.
![](https://github.com/Nov10/SolarSystem/assets/91333241/3bc16e49-1fdb-4756-8f5b-d9a37f8f87a0)

https://github.com/Nov10/SolarSystem/assets/91333241/375a0352-b1a2-480b-9030-f612cecea624

10개의 천체(태양+행성8개+달)를 설정하였으며, 적절한 초기조건을 부여하여 위와 같은 결과물을 만들 수 있습니다.

https://github.com/Nov10/SolarSystem/assets/91333241/b65dcfd4-fde7-4edc-97ef-48fef9fb8834

이건 지구와 달의 모습에 대한 결과물입니다.

그렇다면 몇 가지 의구심을 가질 수 있습니다. **행성 공전 궤도의 이심률은 고려하지 않는건가?** **케플러 법칙은 고려하지 않는건가?** 네, 고려하지 않습니다. 정확이 말하면, **알아서** 고려됩니다.
이심률은 행성 공전 궤도가 적당한 타원의 형태로 돌 수 있도록 초기 속도를 설정하면 알아서 이심률이 계산되며, 천체가 만유인력의 법칙에 따라 움직이도록 계산하면 케플러 법칙은 알아서 구현됩니다. 그러니, 본질적으로 만유인력만 구현해도 충분히 태양계 모형을 만들 수 있습니다.


# 케플러 법칙
그럼 정말로 케플러 법칙이 구현되는지 확인해 봅시다. 케플러의 행성운동법칙은 3개로 구성되는데, 
1. 행성은 모항성을 한 초점으로 하는 타원궤도를 그리면서 공전한다.
2. 행성과 태양을 연결하는 가상적인 선분이 같은 시간 동안 쓸고 지나가는 면적은 항상 같다.
3. 행성의 공전주기의 제곱은 궤도의 긴반지름의 세제곱에 비례한다.

입니다. 간단하게, 제1법칙과 제2법칙만 살펴봅시다.
#### 제1법칙
제1법칙은,  쉽게 말해 행성이 타원으로 이동한다는 뜻입니다. 적당한 초기속도를 설정하면, 이런 결과를 얻을 수 있습니다.

https://github.com/Nov10/SolarSystem/assets/91333241/814a0af4-5995-41c5-8138-7c2ba31e0fec

행성이 타원 궤도로 공전함을 알 수 있습니다.
#### 제2법칙
제2법칙은, 쉽게 말해 시간이 지남에 따라 중심별과 행성을 연결하는 선이 지나간 넓이가 일정하다는 뜻입니다. 즉, 다음과 같이 빗금으로 칠해진 면적을 뜻합니다.
![](https://github.com/Nov10/SolarSystem/assets/91333241/e40fe88e-3f48-4034-b428-d1a199b9d378)
저 면적을 시간이 지남에 따라 계산하여 그래프를 그리면 다음과 같이 나타납니다.
![](https://github.com/Nov10/SolarSystem/assets/91333241/4dac8658-5cb4-4955-9c2d-28bd3391a900)
그래프의 값이 일정한 값으로 나타나고 있음을 알 수 있습니다.
또한, 케플러 제2법칙은 다른 의미가 있는데, 바로 '특히, 행성이 중심별에 가까울수록 빠르게 이동하고 멀 수록 느리게 이동한다'입니다. 이는 위의 영상을 보면 다시 확인할 수 있습니다.

https://github.com/Nov10/SolarSystem/assets/91333241/814a0af4-5995-41c5-8138-7c2ba31e0fec

이렇게 케플러 제2법칙을 확인할 수 있습니다.
다시 강조하자면, 케플러 법칙을 의도적으로 구현하지 않았는데도 케플러 법칙은 알아서 구현된다는 것을 알 수 있습니다.

# 라그랑주 포인트
라그랑주 포인트는 우주 공간에서 작은 천체가 두 개의 큰 천체의 중력에 의해 그 위치를 지킬 수 있는 5개의 위치들을 나타냅니다. 쉽게 말해, 중력이 매우 작아지는 지점을 뜻합니다. 이 라그랑주 포인트를 계산하면, 인공위성이 고정되어 위치할 수 있는 지점을 계산하거나 로켓의 경로를 계산하는 데에도 사용될 수 있습니다.

라그랑주 포인트 또한 엄밀해를 구하는 것이 아닌, 간단하게 5개의 지점 중 1개의 지점(L1)을 2개의 천체에 대해서 풀어 적당한 근사해를 구하도록 하겠습니다.
2개의 천체와 1개의 물체가 있으며, 이 물체의 질량은 무시할 정도로 작습니다. 라그랑주 포인트, 즉 물체가 고정될 수 있는 지점은 두 천체의 중심을 잇는 직선에 존재하며, 이 지점은 두 천체와의 만유인력과 원심력이 평형을 이루는 지점입니다. 즉,

$$\frac{G M_1}{(R - r)^2} - \frac{G M_2}{r^2} = \frac{G (M_1 + M_2)}{R^3} \cdot r$$
이를 정리하면,
$$\frac{M_1}{(R-r)^2}-\frac{M_2}{r^2}=(\frac{M_1}{M_1+M_2}R-r)\frac{M_1+M_2}{R^3}$$
입니다.
이때, $x = \frac{r}{R}, \mu = \frac{M_2}{M_1 + M_2}$라 하면, 위 식은 다음과 같이 $x$에 대한 다항식이 됩니다.

$$x^5 + (\mu - 3)x^4 + (3 - 2\mu)x^3 - \mu x^2 + 2\mu x - \mu = 0$$
이때 두 천체의 질량 차이가 매우 크다면, $\mu=0$으로 근사할 수 있고, 이에 따라 근사 해를 다음과 같이 계산할 수 있습니다.
$$r \approx R \sqrt[3]{\frac{\mu}{3}}$$

이 식을 적절히 변형하면, 한 천체로부터 떨어진 라그랑주 포인트 하나를 계산할 수 있습니다. ```float L1 = r * (1 - Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));```에 해당합니다.
```C#
    private Vector3 CalculateLagrangePointL1(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        float mu = body2.Mass / (body1.Mass + body2.Mass);
        float L1 = r * (1 - Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position + direction * L1;
    }
```

L2, L3인 지점도 다음과 같이 적당한 근사를 이용하여 계산할 수 있습니다.

```C#
    private Vector3 CalculateLagrangePointL2(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        float mu = body2.Mass / (body1.Mass + body2.Mass);
        float L2 = r * (1 + Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position + direction * L2;
    }

    private Vector3 CalculateLagrangePointL3(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position - direction * r;
    }
   ```
 이를 이용하면 3개의 라그랑주 포인트를 계산할 수 있습니다.
 ![Lag1](https://github.com/Nov10/SolarSystem/assets/91333241/e300eb86-466b-4ba4-b016-efaf12d7978d)
![lag2](https://github.com/Nov10/SolarSystem/assets/91333241/91411573-1df0-4d28-be41-7e5dfd5bf620)

https://github.com/Nov10/SolarSystem/assets/91333241/6bc96727-a34a-4e6e-8880-82351a1c0409

빨간색, 초록색, 파란색으로 나타난 3개의 지점이 지구와 달에 대한 라그랑주 포인트들입니다.

다른 행성에 대한 라그랑주 포인트들도 구하면 다음과 같은 결과물을 얻을 수 있습니다.
![Lag3](https://github.com/Nov10/SolarSystem/assets/91333241/7a521b7a-bc46-4bd1-80ec-0f2272117987)

# 라그랑주 포인트 - 3체?
천체가 2개인 상황에서 라그랑주 포인트를 계산하는 것은 꽤나 간단합니다. 힘이 평형을 이루도록 식을 세우고, 적당한 근사를 취하면 됩니다. 그러나 천체가 3개를 넘어가는 순간 문제가 극도로 복잡해지는데, 이는 수학적으로 근사를 한다 해도 그 위치를 계산하기가 매우 어렵습니다.

그렇기에 애초에 다른 방법으로 접근해야 하는데, 그 방법은 계산과학 분야의 가장 핵심이고 중요한 것 중 하나 **수치해석**입니다.
**수치해석**이란 수치적 기법을 통한 해석학적 문제의 근사해를 구하는 것으로, 쉽게 말해 컴퓨터로 수학 문제를 푸는 것입니다. 수치해석을 이용하면 방정식 또한 풀 수 있습니다. 라그랑주 포인트는 다른 천체에 의한 만유인력이 없는, 즉 0이 되는 지점을 뜻하고, 이는 방정식의 형태로 나타남을 알 수 있습니다. 수치해석 방법은 천체의 수에 의존하지 않기 때문에, 3체는 물론, 계산 성능만 뒷받침 된다면 과장 보태서 100체의 라그랑주 포인트도 계산할 수 있습니다.

우리는 결국 방정식을 계산해야 하므로, 방정식의 해를 근사하는 수치해석 방법 중 가장 대표적인 방법인 **뉴턴법**을 사용하여 계산할 것입니다. 이는 다음과 같이 나타낼 수 있습니다.
$$x_{n+1}=x_n-\frac{f(x_n)}{f'(x_n)}$$
자세한 설명은 생략하겠으나, 뉴턴법은 접선의 기울기와 함숫값의 부호에 따라 근이 현재 특정 지점($x_n$)에서 오른쪽 또는 왼쪽에 존재할 것이라고 가정하며 방정식의 해를 근사하는 방법입니다. 이 과정을 충분히 반복하면 $x_n$은 3개 혹은 그 이상의 천체에 대한 라그랑주 포인트가 됩니다. 이는 다음과 같이 간단하게 구현됩니다. ```force / forceDerivative.magnitude```와 ```position -= delta;```가 이에 해당합니다.
```C#    
    private Vector3 CalculateLagrangePoint(Body body1, Body body2, Body body3)
    {
        Vector3 initialGuess = (body1.transform.position + body2.transform.position + body3.transform.position) / 3;
        Vector3 position = initialGuess;
        float tolerance = 1e-4f;
        int maxIterations = 1000;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Vector3 force = Vector3.zero;
            Vector3 forceDerivative = Vector3.zero;

            force += CalculateGravitationalForce(body1, position);
            force += CalculateGravitationalForce(body2, position);
            force += CalculateGravitationalForce(body3, position);

            forceDerivative += CalculateGravitationalForceDerivative(body1, position);
            forceDerivative += CalculateGravitationalForceDerivative(body2, position);
            forceDerivative += CalculateGravitationalForceDerivative(body3, position);

            Vector3 delta = force / forceDerivative.magnitude / 500f;

            position -= delta;

            if (delta.magnitude < tolerance)
            {
                break;
            }
        }

        return position;
    }
```

이를 실행하면, 다음과 같은 결과물을 얻을 수 있습니다. 태양, 지구, 달 사이의 라그랑주 포인트를 계산한 것입니다.

![L3_z_7_3](https://github.com/Nov10/SolarSystem/assets/91333241/e732fe66-9940-4714-87e2-d0d1d07e9650)

https://github.com/Nov10/SolarSystem/assets/91333241/66d1db7d-51b8-47fb-8b32-86dbf7efef0e

https://github.com/Nov10/SolarSystem/assets/91333241/953173be-b2ee-4144-acee-9248c222c6e7

지점이 계속 움직이는 형태인데, 이는 천체가 계속 움직여 해가 불안정하게 계산되기 때문입니다. 천체들의 현재 위치에 따라 라그랑주 포인트가 달라지는 것과 더불어, 뉴턴법의 초기 시작점이 3 천체의 중심점이기 때문에 이러한 문제가 발생합니다. 3체에서의 라그랑주 포인트는 아직까지 알려진 바가 많이 없기 때문에 저 지점이 정확한지는 알 수 없으나, 천체 2개의 라그랑주 포인트와 유사하기에 결과는 일부 비슷하다고 생각할 수 있습니다.

# 값의 스케일
마지막으로, 값의 범위에 대해 논의해야 합니다. 컴퓨터는 유한한 계산 기계이므로 특정 범위 내의 값들만 유효하게 저장할 수 있습니다. 그렇기에 매우 큰 숫자들은 그 값을 나타내기가 어려우며, 이 값들을 적당한 범위 내에 들어오도록 값의 스케일링을 거쳐야 합니다. 이 리포지토리는 다음과 같은 스케일을 사용하였습니다.

### 거리(길이) : 천문단위에 300을 곱하여 계산
### 질량 : 10^23으로 나누어 계산

값을 위와 같이 스케일링하면 질량은 적당히 작은 범위 내에 들어오지만, 거리(길이)에 대해서는 매우 작게 축소시키는 스케일임에도 불구하고 태양에서 해왕성 까지의 거리가 대략 9만정도 됩니다. 이정도면 부동 소수점 오차가 발생할 수준이며, 시각적으로는 볼 수 있을 정도로 그 오차가 있습니다. 그러나 궤도 계산에 있어서는 큰 영향을 미치지는 않기에, 위와 같이 스케일을 조정하여 시뮬레이션을 구현하였습니다.


![Lag3](https://github.com/Nov10/SolarSystem/assets/91333241/7a521b7a-bc46-4bd1-80ec-0f2272117987)
![2](https://github.com/Nov10/SolarSystem/assets/91333241/3bc16e49-1fdb-4756-8f5b-d9a37f8f87a0)
![Capture](https://github.com/Nov10/SolarSystem/assets/91333241/b6374b35-edcb-40a6-92b7-217dd198c271)
![Figure 2024-05-29 094954](https://github.com/Nov10/SolarSystem/assets/91333241/4dac8658-5cb4-4955-9c2d-28bd3391a900)
![Kepler](https://github.com/Nov10/SolarSystem/assets/91333241/e40fe88e-3f48-4034-b428-d1a199b9d378)
![kepler2](https://github.com/Nov10/SolarSystem/assets/91333241/83370b9f-7572-4115-9211-5ab6bed96d5d)
![Lag1](https://github.com/Nov10/SolarSystem/assets/91333241/e300eb86-466b-4ba4-b016-efaf12d7978d)
![lag2](https://github.com/Nov10/SolarSystem/assets/91333241/91411573-1df0-4d28-be41-7e5dfd5bf620)


https://github.com/Nov10/SolarSystem/assets/91333241/375a0352-b1a2-480b-9030-f612cecea624

https://github.com/Nov10/SolarSystem/assets/91333241/66d1db7d-51b8-47fb-8b32-86dbf7efef0e

https://github.com/Nov10/SolarSystem/assets/91333241/953173be-b2ee-4144-acee-9248c222c6e7

https://github.com/Nov10/SolarSystem/assets/91333241/814a0af4-5995-41c5-8138-7c2ba31e0fec

https://github.com/Nov10/SolarSystem/assets/91333241/6bc96727-a34a-4e6e-8880-82351a1c0409

https://github.com/Nov10/SolarSystem/assets/91333241/b65dcfd4-fde7-4edc-97ef-48fef9fb8834
