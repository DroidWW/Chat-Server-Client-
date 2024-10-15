import socket
import os
from os import listdir


with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
    server.connect(("127.0.0.1", 2000))
    data = server.recv(1024).decode('utf-8')
    print(data) 
    while True:
        print('Введите команду:') 
        toSend = input()
        server.send(toSend.encode('utf-8')) 
        data = server.recv(1024).decode('utf-8')
        dataList = data.split('\n') 
        if dataList[0] == "Введите тему":
            print("Введите тему:")
            tema = input()
            c_temi = len(os.listdir("C:\\Users\\Honor\\Desktop\\messages\\"+dataList[1]))
            with open("C:\\Users\\Honor\\Desktop\\messages\\"+dataList[1]+"\\"+str(c_temi+1)+"."+tema+".txt",'w',encoding = 'utf-8') as file:
                print("Введите сообщение:")
                t =""
                while True:
                    message = input()
                    t += message
                    if t.find('.') != -1:
                        break
                    t+="\n"
                w = t.split('.')
                file.write(w[0])
        else:
            if dataList[0] == "Конец программы":
                break
            for msg in dataList: 
                print(msg)
print('Завершение программы')
