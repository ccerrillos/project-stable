<?php
// Routes

$app->get('/dates', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `date` FROM `dates`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($date);
      
      while($stmt->fetch()) {
        array_push($data, $date);
      }

      $stmt->close();
    }
  }

  sort($data);

  return $response->withJson($data);
});

$app->get('/blocks', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `block_id`, `block_name` FROM `blocks`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($block_id, $block_name);

      while($stmt->fetch()) {
        $data[$block_id] = array("block_id" => $block_id, "block_name" => $block_name);
      }

      $stmt->close();
    }
  }

  return $response->withJson($data);
});

$app->get('/grades', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `grade_id`, `grade_name` FROM `grade_levels`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($grade_id, $grade_name);

      while($stmt->fetch()) {
        $data[$grade_id] = array("grade_id" => $grade_id, "grade_name" => $grade_name);
      }

      $stmt->close();
    }
  }

  return $response->withJson($data);
});

$app->get('/houses', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `house_id`, `house_name` FROM `houses`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($house_id, $house_name);

      while($stmt->fetch()) {
        $data[$house_id] = array("house_id" => $house_id, "house_name" => $house_name);
      }

      $stmt->close();
    }
  }

  return $response->withJson($data);
});

$app->get('/locations', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `location_id`, `location_name` FROM `locations`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($location_id, $location_name);

      while($stmt->fetch()) {
        $data[$location_id] = array("location_id" => $location_id, "location_name" => $location_name);
      }

      $stmt->close();
    }
  }

  return $response->withJson($data);
});

$app->get('/presentations', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `presentation_id`, `first_name`, `last_name`, `topic`, `date`, `block_id`, `location_id` FROM `presentations`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($presentation_id, $first_name, $last_name, $topic, $date, $block_id, $location_id);
      $stmt->store_result();
      while($stmt->fetch()) {
        $data[$presentation_id] = array(
          "presentation_id" => $presentation_id,
          "first_name" => $first_name,
          "last_name" => $last_name,
          "topic" => $topic,
          "date" => $date,
          "block_id" => $block_id,
          "location_id" => $location_id
        );
      }

      $stmt->close();
    }
  }

  return $response->withJson($data);
});

$app->get('/viewers', function($request, $response, $args) {
  $data = array();
  
  $q = "SELECT `viewer_id`, `firstn_aame`, `last_name`, `house_id`, `grade_id` FROM `viewers`;";

  if($stmt = $this->db->prepare($q)) {
    if($stmt->execute()) {
      $stmt->bind_result($viewer_id, $first_name, $last_name, $house_id, $grade_id);
      $stmt->store_result();
      while($stmt->fetch()) {
        $data[$viewer_id] = array(
          "viewer_id" => $viewer_id,
          "first_name" => $first_name,
          "last_name" => $last_name,
          "house_id" => $house_id,
          "grade_id" => $grade_id
        );
      }

      $stmt->close();
    }
  }

  return $response->withJson($data);
});