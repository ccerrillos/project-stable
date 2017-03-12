<?php
// DIC configuration

$container = $app->getContainer();
/*
// view renderer
$container['renderer'] = function ($c) {
    $settings = $c->get('settings')['renderer'];
    return new Slim\Views\PhpRenderer($settings['template_path']);
};
*/
// monolog

$container['logger'] = function ($c) {
    $settings = $c->get('settings')['logger'];
    $logger = new Monolog\Logger($settings['name']);
    $logger->pushProcessor(new Monolog\Processor\UidProcessor());
    $logger->pushHandler(new Monolog\Handler\StreamHandler($settings['path'], $settings['level']));
    return $logger;
};

$container['db'] = function($c) {
  if(isset($_SERVER['RDS_HOSTNAME'])) {
    $db_host = $_SERVER['RDS_HOSTNAME'];
    $db_port = $_SERVER['RDS_PORT'];
    $db_user = $_SERVER['RDS_USERNAME'];
    $db_pass = $_SERVER['RDS_PASSWORD'];
    $db_name = $_SERVER['RDS_DB_NAME'];
  }

  $mysqli = new mysqli($db_host, $db_user, $db_pass, $db_name, $db_port);
  return $mysqli;
};