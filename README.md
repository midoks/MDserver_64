# MDserver_64

### 说明
一个款在Windows7,8,10 64位集成的PHP环境,一般WIN8,WIN10不带.NET,需要安装

### 详情
```
MDserverV3(PHP环境一键集成),文件采用7-Zip压缩,将解压缩到一个路径中不含有汉字和空格的分区或目录即可。
这是64位版本的。
     
1.MDserver集成了一下软件:
PHP5.5.29(TS) | PHP7.1(TS)
Apache2.4.7
MySQL 5.6.24
memcached
redis
MongDB

Rythem			HTTP抓包工具
Putty			SSH登录工具
WinSCP			SSH登录工具
FlashFxp		FTP登录工具
VIM			代码编辑器
kcachegrind		Xdebug分析器
SecureCRT		ssh登陆
HeidiSQL		数据库管理
FSCapture		截图工具

2.PHP集成yaf框架,lumen,phalcon,php_apm,
php_apm,php_bitset,php_seaslog,php_solr,
php_ssh2,php_yaml
php_sphinx,php_varnish
等必备的扩展和开发工具

注意：
1.MySQL默认用户名：root，密码为空
2.MySQL数据库文件存放目录：MySQL\data


MySQL后台:
localhost/phpMyAdmin
127.0.0.1/phpRedisAdmin

Redis后台:
localhost/phpRedisAdmin
或
127.0.0.1/phpRedisAdmin

Mongodb后台:
localhost/phpMongodb
或
127.0.0.1/phpMongodb

3.新增php版本切换功能(在php设置下面)

4.调试
127.0.0.1/info.php?mdd=ok

5.imagick扩展集成
安装文件在bin文件下,也可以自己下载:http://www.imagemagick.org/download/binaries/
安装后,需要重新启动电脑。重启电脑后,生效

一些问题:
1.第一启动的时候,会遇到一些阻碍,比如360的安全警报。
2.都要选择线程安全的php版本
3.在使用过程中遇到Win10默认占用80端口,解决Win10默认占用80端口.
(http://jingyan.baidu.com/article/08b6a591a31d8914a8092214.html)


- httpd.exe你的电脑中缺失msvcr110.dll
http://www.microsoft.com/en-us/download/details.aspx?id=30679

- 应用程序无法正常启动0xc000007b解决方法
http://jingyan.baidu.com/article/4dc408488ff783c8d946f1e8.html

```

###相关链接
- [v3.8.0.0](https://pan.baidu.com/s/1gfm03IV)
 * 添加web管理界面
 * FSCapture软件更新
 * 代码优化

- [v3.7.9.1](https://pan.baidu.com/s/1bpy5gIj)
 * memcache.php工具添加
 * memcache(www/cmd)启动bat
 
- [v3.7.9](http://pan.baidu.com/s/1dE6qDQL)
 * php71版本增加memcache扩展

- [v3.7.8](http://pan.baidu.com/s/1kVuit4j)
 * 减少php70版本维护
 * 增加一些php71版本扩展

- [v3.7.7](http://pan.baidu.com/s/1bpCDuk3)
 * 修复xhprof不能使用的问题(官方的存在问题)
 
- [v3.7.6](http://pan.baidu.com/s/1o80LnMq)
- [v3.7.5](http://pan.baidu.com/s/1pLnQ3az)
- [v3.7.4](http://pan.baidu.com/s/1o8FBMEA)
- [v3.7.3](http://pan.baidu.com/s/1bpcczwj)
- [v3.7.2](http://pan.baidu.com/s/1nvGrB4l)
- [v3.7](http://pan.baidu.com/s/1kV2izmJ)

### 截图

- Main
[![截图](/images/screen_1.png)](/images/screen_2.png)

- Web管理
[![截图](/images/screen_2.png)](/images/screen_2.png)
