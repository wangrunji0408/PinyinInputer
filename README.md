计53 王润基 2015011279

2017.04.04

## 目录说明

* `bin`：可执行文件
  * `ConsoleApp.dll`：命令行程序
  * `PinyinAnalyzer.config.json`：配置文件
* `data`：
  * `text`：样例语料文件夹
  * `stat`：统计信息文件夹
  * `model`：模型数据文件夹
  * `test`：测试数据文件夹
    * `input.txt`：测试文件输入。每行是一个拼音序列。
    * `output.txt`：测试文件输出。每行是中文序列。
    * `mytest_in.txt`：测试集。每行是中文序列。
    * `mytest_out.txt`：测试结果。显示特定模型在测试集上的表现。
    * `official_test_in.txt`：官方测试集（2017.04.06）
    * `official_test_out.txt`：官方测试集结果
* `src`：工程文件夹

## 使用说明

本程序使用C#语言编写，运行环境是.Net Core，可跨平台运行。

* 运行环境：.Net Core 1.1 命令行工具
* 开发环境：Visual Studio（for Mac）/ Rider
* 运行方法：进入`bin`文件夹，使用`dotnet ConsoleApp.dll` 运行，此时会显示帮助文档。

### 标准流程

```mermaid
graph LR
A[样本文件] --> B[样本词频]
B --> C[统计语言模型]
C --> D[输入法]
```

1. 分析数据集，统计子串频率。
2. 根据词频，生成语言模型。
3. 根据语言模型，实现拼音输入法的功能。

### 使用样例

1. 分析三个样本文件，分别在旁边生成统计文件

   **注：样本文件必须是UTF-8编码**

   ```bash
   dotnet ConsoleApp.dll analyze ../data/text/三体1.txt ../data/text/三体2.txt ../data/text/三体3.txt -d ../data/stat/
   ```

   在data/stat文件夹下生成了3个`三体*.txt_stat.csv`文件

2. 合并统计文件

   ```bash
   dotnet ConsoleApp.dll merge ../data/stat/三体1.txt_stat.csv ../data/stat/三体2.txt_stat.csv ../data/stat/三体3.txt_stat.csv -o ../data/stat/三体_stat.csv
   ```

   在data/stat文件夹下生成了`三体_stat.csv`文件

3. 生成语言模型文件

   *注：为了保证质量，这里使用已经统计好的`sinanews_stat.csv`*

   ```bash
   dotnet ConsoleApp.dll build ../data/stat/sinanews_stat.csv -m 1 2 3 n
   ```

   在data/model文件夹下生成了4个语言模型文件：`NGram*Model.txt`

4. 使用语言模型构建输入法

   1. 文件输入输出：（**作业要求内容**）

      ```bash
      dotnet ConsoleApp.dll solve ../data/test/input.txt ../data/test/output.txt
      ```

   2. 交互式测试：

      ```bash
      dotnet ConsoleApp.dll qsolve
      ```

      在命令行中一次输入一行拼音，输出每步的前几个最优解和概率，以及最终解。

      在不指定模型时，默认使用【基于字的动态n-gram模型】。

      也可以指定模型，格式为：

      ```bash
      dotnet ConsoleApp.dll qsolve [ModelName=1/2/3/n/12m/12l/123l/]
      ```

5. 测试输入法的效果

   1. 纯中文输入

      ```bash
      dotnet ConsoleApp.dll test ../data/test/mytest_in.txt ../data/test/mytest_out.txt -f chinese_only -m 1 12m 12l 123l n
      ```

      输入文件：测试集。每行一个中文串，不能有其它字符（数字，标点，空格）

      打开`data/test/mytest_out.txt`查看分析结果。

   2. 拼音、中文交替输入（官方测试集格式）

      ```bash
      dotnet ConsoleApp.dll test ../data/test/official_test_in.txt ../data/test/official_test_out.txt -f pinyin_chinese -m 1 12l n
      ```

      打开`data/test/official_test_out.txt`查看分析结果。

6. 分析

   此命令行程序集成了若干交互式数据分析查询工具

   1. 查询拼音数据库

      ```bash
      dotnet ConsoleApp.dll qpinyin
      ```

      ```bash
      pinyin > pin
      品姘嫔拚拼榀牝聘贫频颦
      pinyin > 拼
      pin
      ```

   2. 查询样本统计数据库

      ```bash
      dotnet ConsoleApp.dll qstat ../data/stat/sinanews_stat.csv
      ```

      ```bash
      stat > *
      702739202
      stat > 中华人民共和国
      9270
      ```

   3. 查询语言模型数据库概率信息

      ```bash
      dotnet ConsoleApp.dll qmodel n
      ```

      ```bash
      data > 清华大学 ji
      1-gram
      机(0.2529016)
      记(0.2444878)
      其(0.1869959)
      期(0.1580854)
      及(0.1575293)

      2-gram
      技(0.3125243)
      期(0.3122525)
      系(0.1399565)
      籍(0.1333566)
      基(0.1019101)

      3-gram
      期(0.483188)
      计(0.4408468)
      机(0.07596512)

      n-gram
      期(0.483188)
      计(0.4408468)
      机(0.07596512)
      ```

## 工程文件说明

工程文件夹位于`src/PinyinAnalyzer`，有如下子项目：

- `ConsoleApp`：控制台应用。为功能提供简单的命令行接口
- `UnitTest`：单元测试。及若干开发时的应用函数

* `PinyinAnalyzer`：类库。实现全部业务功能

  * `Base`：基础设施类

    * `PinyinDict`：拼音词典类。单例模式。

      读取拼音数据文件，实现字符/拼音双向互查。

    * `Statistic`：统计类。

      键-频率。使用字典朴素实现，可存取到csv文件。

    * `Distribute`：概率分布类。

      键-概率。保证概率和为1。可从`Statistic`生成。

    * `Condition`：条件类。表示条件概率中的条件。

  * `TextStatistician`：文本统计器

    读取文本，使用`Statistic`类统计子串频率

    实现了两种统计策略：

    * 统计所有长度<=k的子串
    * 统计所有高频子串，动态调整子串长度

  * `NGramModel`：若干N-Gram语言模型

    * `NGram[i]Model`：i=1/2/3。针对性地使用数据结构实现。

    * `NGramNModel`：使用string-Distribute字典实现的通用NGram模型。

    * `NGramBindModel`：结合多种不同阶模型

      实现了两种概率分布混合策略：

      * 取有效数据中阶最大的
      * 线性混合

    * `NGramModelFileLoader`：模型的文件保存和读取

      使用JSON序列化/反序列化

  * `Inputer`：基于上述模型的输入法

  * `InputerTester`：输入法效果测试器