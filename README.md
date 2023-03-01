# 개발 계획

### 1단계 - Basic 단순 로직 개발 (진행중)
- agent에서 룰  
- 프로그램 차단, 실시간 실행중인 앱 리스트 전송
- 알림 - 배치   
  - 정 시간마다 사용자 현황 (켜져있는 agent, 실행중인 프로그램 목록)
  - 단 로그 수신 시 알림

### 2단계 - 룰 분석 엔진 도입 (예정)
- batch, rule engine, Flunt bit
- 여러 룰 추가, 이벤트 로그 등등

### 3단계 - 빅데이터 기반 처리 (예정)
- kafka, elk
- hdfs, spark streaming - 실시간 및 데이터 저장
- 에이전트 다양화

</br>

# 1단계 - Basic

</br>

# Agent 

</br>

## 1.  기능 

- 에이전트 프로그램 서버 등록 
  - 해당 에이전트 설치정보를 서버에 전송하여 등록 
- 차단 프로그램 리스트 수신 
  - Agent 처음 시작 시 서버에 요청하여 수신  
  - 처음 시작 후 Schedule 에 따라 서버에 요청

- Agent 실행 중인 프로세스 리스트 전송
  - 이전 프로세스 리스트와 비교 후 업데이트 된 경우 해당 내역 전송 

- 차단된 프로그램 감시 기능
  - 1초마다 차단된 리스트의 프로세스 확인
  - 해당 프로그램 종료
  - 조치 결과 전송 

## 2.  구조

- 윈도우 서비스 형태 개발
- MSI 설치 형태 지원


</br>

# Master

</br>

## 1.  기능 

- Alram Batch 
  - Server에서 로그 수신 시, 알람 기본 테이블 바탕으로 알림 배치 수행 (메일 OR 문자)
  - 특정 시간마다, 종합 현황 알림 배치 처리

</br>

- Server 
  - 에이전트 등록
  - 차단 리스트 발송
  - 차단 로그 수신
  - agent helath(프로세스 상태) check 기능)

</br>

- View
  - 관리자 로그인, 등록
  - 그룹 추가, 관리
  - 그룹 별 불 추가, 관리
  - 실시간 에이전트 현황 관리
  - 에이전트 관리
  - 로그 view


## 2.  구조
- Alram Batch : spring batch
- server : spring mvc, jpa
- view : react
- db : postgreSQL


</br>

## DATABASE 

![image](https://github.com/ktn1075/IntegratedLogSolution/blob/main/DB%20%EC%84%A4%EA%B3%84.png)


</br>

## API 설계 

1. 에이전트 등록 API(agent 정보 조회, 신규등록)
 - Reqeust 
   - body
    - type : json 
    - filed : hMac[암호화된 맥 주소], alias[별칭]
 - Response
   - header
      - status : 성공 200, 잘못된요청 400, 차단된사용자 403
   - body 
      - type : json 
      - filed : agentId, groupId 

2. 정책 수신 API : 
 - Reqeust 
   - body
      - type : json 
      - filed : hMac[암호화된 맥 주소], ruleVer, agentId, groupId 
 - Response
   - header
      - status : 성공 200, 잘못된요청 400, 룰 버전 동일시 204, 차단된사용자 403  
   - body 
      - type : json 
      - filed : rules[{ruleId,ruleNm,ruleType,ruleVer,content}] 차단 rule 경우 programNm[]

3. 로그 수신 API :
 - Reqeust 
   - body
      - type : json 
      - filed : hMac[암호화된 맥 주소], ruleVer, ruleId, ruleType, alertTm, content, agentId, groupId,
 - Response
   - header
      - status : 성공 200, 잘못된요청 400

4. agent health(에이전트 상태) check api 
 - Reqeust 
   - body
      - type : json 
      - filed : hMac[암호화된 맥 주소], agentId, groupId, alias[별칭], processList[]
 - Response
   - header
      - status : 성공 200, 잘못된요청 400
</br>

## 제한사항 
-  Agent -> Master 단방향 지원
   - Rest/Api 방식으로 마스터와 통신하기에 내부 유동 IP를 사용하는 Agent에게 서버가 먼저 통신하기에 제한됨.

</br>

## 도구 
- 프로젝트관리 : JIRA 
- 패키징 : WIX 
- IDE : VisualStudio, Intelij
- DBMS : ?
- 업무협업툴 : Slack

