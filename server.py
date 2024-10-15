import socket
import os
from os import listdir

bExit = False
Authed = True
login = "" 
pt = "C:\\Users\\Honor\\Desktop\\messages\\"
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(("127.0.0.1", 2000))
server.listen(4)
print("Ожидание клиента...")
clientSocket, address = server.accept()
clientSocket.send("Подключение к серверу прошло успешно!".encode("utf-8"))
print('Клиент подключился')

def getNames():
    files = listdir(pt)
    for usr in open('pass.txt', 'r',encoding = "utf-8").readlines():
        stored = usr.split(' ')
        kok = pt + stored[0]
        if not os.path.exists(kok):
            os.makedirs(kok)

getNames()   
def getMessages():
    temi = listdir(pt + login)
    if len(temi) ==0:
        print(len(temi))
    t = ""
    for tema in temi:
        t +="\n"
        t += "тема: "+ tema.removesuffix(".txt") + "\n"
        nm = pt + login+ "\\"+ tema
        for usr in open(nm, 'r',encoding = "utf-8").readlines():
            t+=usr
    clientSocket.send(t.encode('utf-8'))
        
def cmdError(errorNum):
    print('Ошибка', errorNum)
    errors = {
        1: "Незвестная комманда! Введите 'help' чтобы получить список доступных команд.\n",
        2: "Неверное количество параметров!\n",
        3: "Неверное имя пользователя или пароль!\n",
        4: "Вы уже авторизованы!\n",
        5: "Для использования этой команды для начала необходимо авторизоваться!\n",
        6: "Сообщений нет"
    }
    msg = errors[errorNum]
    print(msg) 
    clientSocket.send(msg.encode("utf-8"))

def cmdAuth(data):
    print('Вызов auth')
    global Authed
    if len(data) != 3:
        cmdError(2)
        return
    if not Authed:
        cmdError(4)
        return
    for usr in open('pass.txt', 'r',encoding = "utf-8").readlines():
        stored = usr.split(' ')
        if (data[1] == stored[0] and data[2] == stored[1]):
            clientSocket.send("Вы успешно авторизованы!\n".encode("utf-8"))
            global login
            login = stored[0]
            Authed = False
            break

def cmdList():
    print('Вызов list',Authed)
    if Authed: 
        cmdError(5)
        return
    lol = data.split(' ')
    if len(lol) > 1:
        cmdError(2)
        return
    toSend = "Сообщения пользователя:\n"
    if len(listdir(pt+login))==0:
        return
    getMessages()

def cmdRead(msg):
    print('Вызов read')  
    if Authed: 
        cmdError(5)
        return
    if len(msg) != 2:
        cmdError(2)
        return
    np = pt + login 
    c = len(os.listdir(np))
    d = os.listdir(np)
    v = d[int(msg[1])-1]
    if int(msg[1]) <= c:
        t =""
        for usr in open(np+"\\"+v,'r',encoding = 'utf-8').readlines():
            t += usr
        clientSocket.send(t.encode("utf-8"))
    
    
def cmdSend(msg):
    print('Вызов send')
    if Authed:
        cmdError(5)
        return
    if len(msg) != 2:
        cmdError(2)
        return
    tema = ""
    clientSocket.send(("Введите тему\n"+msg[1]).encode("utf-8"))

    
def cmdExit():
    print('Вызов exit')  
    global bExit
    bExit = True
    clientSocket.send("Конец программы".encode("utf-8"))
 
def cmdHelp():
    print('Вызов help')  
    clientSocket.send("list - выводит список сообщений пользователя\n" \
                      "auth <имя пользователя> <пароль> - авторизация\n" \
                      "send <имя пользователя>  - отправляет сообщение пользователю\n" \
                      "read <номер сообщения> - выводит сообщение авторизованного пользователя под номером\n" \
                      "exit - выход из программы\n".encode("utf-8"))

def checkCommand(data):
    lol = data.split(' ')
    match lol[0]:
        case "list":
            cmdList()
        case "auth":
            cmdAuth(lol)
        case "read":
            cmdRead(lol)
        case "send":
            cmdSend(lol)
        case "exit":
            cmdExit()
        case "help":
            cmdHelp()
        case _:
            cmdError(1)

while not bExit:
    data = clientSocket.recv(1024).decode('utf-8')
    checkCommand(data)

print('Сервер закрылся...')
server.close()
