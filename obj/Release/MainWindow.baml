   M S B A M L   `   `   `  ���� F  BProxyTester, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:ProxyTesterProxyTester  E ADragablz, Version=0.0.3.203, Culture=neutral, PublicKeyToken=null5(clr-namespace:Dragablz;assembly=DragablzDragablz )clr-namespace:ProxyTesterProxyTester      ProxyTester.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:ProxyTester     =dragablz(clr-namespace:Dragablz;assembly=Dragablz     V RMaterialDesignThemes.Wpf, Version=3.1.1.1361, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  (Controlsclr-namespace:ProxyTester  -      MaterialDesignPaper#���     PrimaryHueLightBrush#���    � Title$  Trek Proxy Tester�� � Icon$ img/Icon1.ico�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ SingleBorderWindow=�$��0q�$
��1010�� � 	Resources  �� 
TargetType �� Property �� Value��%2   '��       �� " ��.���  � ��$ Stretch=�.��� .+���  �� Orientation$ Vertical=�.+�#  MaterialDesignThemes.Wpf.Card  $��0q�.���� $��img/logo.png��$	��150��$
��3350��� -   	 �Name$	 
gridHeight��.+�  Dragablz.TabablzControl  -   $��17(�
   FixedHeaderCount$
 3��$��0q�.@��� -    ��Name$ tabproxy��$��PROXIES��.���� $ Vertical=�$��20,10q�.+��  � RowDefinitions �� $��1* ��� -    ��Name$ rowTab1Height���� $��1* � � ColumnDefinitions �� $��3* ��� $��2* �.+���  $  MaterialDesignHeadline6TextBlock#���  $��0��$��0��
���� ��$��Center=�	Proxies�� -     MaterialDesignRaisedButton#���  $��0��$��0��$��IMPORT PROXY��$��Right=�$��	0,0, 10,0q��� #���  $��0��$��1��
���� ��$��Center=�	New Proxy  $��5q�$��10, 10q�$��1��$��0��.���� -    ��Name$ 
lv_proxies��$��Visible=�$��0, 10q� �� View ��  �� AllowsColumnReorder .  �� ColumnHeaderToolTip$ 
Proxy List��.P��� $��IP�� �� DisplayMemberBinding ��  �� Path$ ip��� $
��Port�� �� $
 port��� $��	User Name�� �� $ username��� $��Password�� �� $ password��� $��Status�� �� CellTemplate �� .R��� .+��� -   $
��0,-7q�$	��5,0q�$��10(�
������ $��0q�
 � Tag �� *id+���� *
btnContent+���� *color+  $��5q�$��10, 10q�$��1��$��1��.���  �� $��9* ��� $��1* � �� .+��� -	    ��Name$ tb_newproxy��$��0��$��0��$��10q� �� TextWrapping$
 Wrap=� �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=��� -
   #���  $
��35,0q�$��	ADD PROXY��$��1��$��0��$	��120���� -   $ tabsite��$��SITES��.���� $ Vertical=�$��20,10q�.+��  �� $��* ��� -   $ rowTab2Height���� $��* � �� $��3* ��� $��1* �.+��� #���  
���� ��$��0��$��0��$��Center=�	All Sites�� #���  
���� ��$��0��$��1��$��Center=�
New Site  $��5q�$��10, 10q�$��1��$��0��.���� -   $ lv_sites��$��Visible=�$��0, 10q�  MaterialDesignListView#���   ��  . $ 	Site List��.P���  �� Width$	 120��$��Check�� �� .R��� .+���  d� 	IsChecked ��  �� RelativeSource ��  �� AncestorType$ ListViewItemL�$ 
IsSelected��� $��	Site Name��$	 150�� �� $ sitename��� $��Site Domain��$	 230�� �� $ 
sitedomain�  $��
10,10,0,10q�$��15q�$��1��$��1��.���  �� �� �� ��  �� .+��� $ 
Horizontal=�$��0, 10q�$��1��.+��� $��15(�
���� ��Domain Url :�� -   $ 	tb_domain��  MaterialDesignTextBox#���  $	��Top=�$	��350��$��17(�$��	10,-5,0,0q��� $ 
Horizontal=�$��0, 10q�$��2��.+��� $��15(�
���� ��$��1��$��Center=�Site Name :�� -   $ tb_siteName��#���  $	��Top=�$	��355��$��17(�$��	10,-5,0,0q��� -   #���  $��30, 10, 30, 30q�$��2��$��0��$	��100��$��DELETE���� -   #���  $��30, 10, 30, 30q�$��2��$��1��$	��100��$��ADD SITE���� -   $ tabproxytest��$
��TEST��.���� $ Vertical=�.+��  �� $��1* ��� -   $ rowTab3Height��$	��200 ��� $��1* � �� $��* �.+�� $��35, 5q�$��0��$��0�� ��  �� $��3* ��� $��1* �.+��� $ 
Horizontal=�$��0��$��0��.+��� $��Select Site�����#673AB7   MaterialDesignBody1TextBlock#���  �� -    ��Name$ cb_sites��$��15(�  MaterialDesignComboBox#���  $��	20,0,20,0q�$	��150���� $ 
Horizontal=�$��Right=�$��0��$��1��.+��� $��Speed�����#673AB7#���  �� -   $ tb_ms��$��40��$��14(�$��	10,3,10,0q�#���  �� $��ms�����#673AB7#���    $��5q�$��30, 5q�$��1��$��0��.���� -   $ lv_proxytests��$��Visible=�$��0, 10q� ��  . $ 	Test List��.P��� $��Proxy��$	 210�� �� .R��� .+��� ���� $ proxy����� *color+�� $��	User Name��$	 210�� �� .R��� .+��� ���� $ username����� *color+�� $��Password��$	 210�� �� .R��� .+��� ���� $ password����� *color+�� $��Status��$	 155�� �� .R��� .+��� ���� $ status�  �� NotifyOnTargetUpdated  . ���� *color+�� $��
Speed (ms)��$	 100�� �� .R��� .+��� y��� $ speedhidden����� $ speed����� *color+�  % !MaterialDesignCircularProgressBar#��[ $��2��! � IsIndeterminate! . y��� $ progresshidden�� $��2��$��0�� ��  �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� -   #���  $��30, 10q�$��0��$��TEST ALL���� -   #���  $��30, 10q�$��1��$��DELETE FAILED���� -   #���  $��30, 10q�$��2��$��RELOAD���� -   #���  $��30, 10q�$��3��$��EXPORT���� -   #���  $��30, 10q�$��4��$
��COPY��