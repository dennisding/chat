
# 小聊天

一个用C#编写的小聊天服务器

## 怎么用?

编译后直接运行ChatServer和ChatClient, 目前只能连接本地服务器, 想连接其它服务器请改源码里的ip地址.
可以启动一个ChatServer和多个ChatClient

## 具体指令

登录, 目前判断用户名和密码一致表示登录成功
'''
login {userName} {password}
'''

创建新聊天室
'''
new {roomName}
'''

离开聊天室
'''
leave
'''

进入聊天室
'''
enter {roomName}
'''

发送聊天室公开消息
'''
msg msg_to_ chat room
'''

发送消息给具体的人
'''
to {userName} msgxxx
'''

打印帮助信息
'''
help
'''

退出客户端
'''
exit|quit
'''
