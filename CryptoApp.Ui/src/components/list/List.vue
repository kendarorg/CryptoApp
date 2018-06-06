<template>
	<div class="row" style="width:100%;">
		<!-- "00000000-0000-0000-0000-000000000000" -->
		<div class="col col-lg-6 fileLoad pre-scrollable"">
			<div  class="row" style="width:100%;">
				<div class="form">
					<div class="form-group">
						<label for="search">New:</label>
						<input class="form-control" id="search" type="text" size="30" 
							v-model="model.search" />
					</div>
					<div class="form-group">
						<input class="form-control" type="button" value="Search" @click="loadTree()">
					</div>
				</div>
			</div>
			<div  class="row" style="width:100%;">
				<ul>
					<li v-for="node in model.nodes">
						<span @click="onClick(node)">{{node.label}}</span>
					</li>
				</ul>
			</div>
		</div>
		<div class="col col-lg-6 fileLoad">
			<component :is="currentView" 
				:parentData="model.nodeId" :toMove="model.toMoveId" :action="model.action"></component>
		</div>
	</div>
</template>
<script>
	import treeController from '@/components/tree/treeController'
	import Vue from 'vue'
	import {EventBus} from '@/services/busService.js'
	
	
	
	export default {
		components:{
			//TreeMenu
		},
		name:'tree',
		mounted(){
			// Listen to the event.
			EventBus.$on('treeEvent', this.treeEventHandler);
		},
		beforeDestroy(){
			// Stop listening.
			EventBus.$off('treeEvent', this.treeEventHandler);
		},
		data() {
			return  {
				
				model:{
					search:'',
					nodes:[],
					action:'show'
				},
				isEditing:false,
				currentView : 'tree-none'
			}
		},
		methods:{
			treeEventHandler:function(evt){
				var context = this;
				//var node = this.model.node;
				if(evt.event){
					console.log("List "+evt.event);
					if(evt.event=='startEditing'){
						context.isEditing =true;
					}else if(evt.event=='doLoad'){
						context.currentView = "tree-none";
						
						setTimeout(()=>{
							context.currentView = context.loadItemView(evt.id,evt.isFolder);
							context.model.nodeId=evt.id;
							context.model.action="show";
							context.isEditing =false;
						},200);
					}if(evt.event=='doDelete'){
						context.currentView = "tree-none";
					}
					this.$forceUpdate();
				}
			},
			
			loadItemView:function(item,isFolder){
				return "tree-leaf";
			},
			onClick:function(clicked){
				if(this.isEditing)return;
				var context = this;
				context.currentView = "tree-none";
				setTimeout(()=>{
					context.currentView = context.loadItemView(clicked.id,clicked.isFolder);
					context.model.nodeId=clicked.id;
					context.model.isFolder=clicked.isFolder;
					console.log("clicked: "+clicked.label);
				},200);
			},
			loadTree:function(){
				var context = this;
				
				while(context.model.nodes.length>0){
					context.model.nodes.pop();
				}
				
				treeController.doSearch(context.model.search,function(data){
					while(data.length>0){
						context.model.nodes.push(
							context.buildItem(data.pop())
						);
					}
				},null);
			},
			buildItem:function(data){
				var context = this;
				return {
					onClick:context.onClick,
					blocked:function(){
						return context.isEditing;
					},
					id:data.Id,
					label:data.Title,
					isFolder:data.IsFolder//,
					//nodes:[]
				}
			}	
		}
	}
</script>