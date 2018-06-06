<template>
	<div width="100%">
	<div class="row" style="width:100%;">
		<div class="col col-lg-6">
			<a href="/api/tree/backup">
				Download Xml Backup for KeePass
			</a>
		</div>
	</div>
	<div class="row" style="width:100%;">
		<!-- "00000000-0000-0000-0000-000000000000" -->
		<div class="col col-lg-6 fileLoad pre-scrollable"">
			<tree-menu
				:label="model.node.label" 
				:nodes="model.node.nodes"
				:node="model.node"
				:depth="0"></tree-menu>
		</div>
		<div class="col col-lg-6 fileLoad">
			<component :is="currentView" 
				:parentData="model.nodeId" :toMove="model.toMoveId" :action="model.action"></component>
		</div>
	</div>
	</div>
</template>
<script>
	import treeController from '@/components/tree/treeController.js'
	import Vue from 'vue'
	import {EventBus} from '@/services/busService.js'
	
	
	
	export default {
		components:{
			//TreeMenu
		},
		name:'tree',
		mounted(){
			this.loadTree();
			console.log("BUILT TREE VUE");
			
			// Listen to the event.
			EventBus.$on('treeEvent', this.treeEventHandler);
		},
		beforeDestroy(){
			// Stop listening.
			EventBus.$off('treeEvent', this.treeEventHandler);
		},
		data() {
			return  {
				whoIsMoving:null,
				model:{
					rootId: '',
					node:{
						isExpanded:false
					},
					nodeId:'',
					toMoveId:'',
					isFolder:false,
					action:'show'
				},
				isEditing:false,
				currentView : 'tree-none'
			}
		},
		methods:{
			treeEventHandler:function(evt){
				var context = this;
				var node = this.model.node;
				if(evt.event){
					console.log("Tree "+evt.event);
					/*if(evt.event=='doRebuild'){
						if(node.isExpanded){
							treeController.loadChildrenById(evt.id,function(data){
								var expanded = [];
								for(var i=0;i<node.nodes.length;i++){
									if(node.nodes[i].isExpanded){
										expanded.push(node.nodes[i].id); 
									}
								}
								context.rebuildChildren(node,data);
								for(var i=0;i<expanded.length;i++){
									EventBus.$emit('treeEvent',{
										event:'doRebuild',
										id:expanded[i]
									});
								}
							});
						}else{
							while(node.nodes.length){
								node.nodes.pop();	
							}
						}
					}else*/ if(evt.event=='doMoveToRoot'){
						context.currentView = "tree-none";
						
						setTimeout(()=>{
							this.whoIsMoving=(null);
							this.loadTree();
						},200);
					}else if(evt.event=='startEditing'){
						context.isEditing =true;
					}else if(evt.event=='doLoad'){
						context.currentView = "tree-none";
						
						setTimeout(()=>{
							this.whoIsMoving=(null);
							context.currentView = context.loadItemView(evt.id,evt.isFolder);
							context.model.nodeId=evt.id;
							context.model.action="show";
							context.isEditing =false;
						},200);
					}else if(evt.event=='doAddLeaf'){
						
						context.currentView = "tree-none";
						
						setTimeout(()=>{
							context.isEditing =true;
							console.log("addChildLeaf to "+evt.parentId);
							context.currentView = context.loadItemView(evt.parentId,false);
							context.model.nodeId=evt.parentId;
							context.model.toMoveId=evt.parentId;
							context.model.action="new";
						},200);
					}else if(evt.event=='doAddNode'){
						context.currentView = "tree-none";
						setTimeout(()=>{
							context.isEditing = true;
							console.log("addChildNode to "+evt.parentId);
							context.currentView = context.loadItemView(evt.parentId,true);
							context.model.nodeId=evt.parentId;
							context.model.toMoveId=evt.parentId;
							context.model.action="new";
						},200);
					}else if(evt.event=='doDelete'){
						context.currentView = "tree-none";
					}else if(evt.event=='doMove'){
						console.log("move "+evt.id);
						this.whoIsMoving=(evt.id);
						context.currentView = context.loadItemView(evt.id,true);
						context.model.nodeId=evt.id;
						context.model.toMoveId=evt.id;
					}else if(evt.event=='doCancelMove'){
						
						context.currentView = "tree-none";
						
						setTimeout(()=>{
							this.whoIsMoving=(null);
							context.currentView = context.loadItemView(
								context.model.nodeId,
								context.model.isFolder);
							context.model.nodeId=context.model.nodeId;
							context.model.isFolder=context.model.isFolder;
							context.model.toMoveId='';
						},200);
					}
					this.$forceUpdate();
				}
			},
			
			loadItemView:function(item,isFolder){
				if(this.whoIsMoving!=null){
					return "tree-move";
				}
				if(isFolder){
					return "tree-node";
				}
				return "tree-leaf";
			},
			onClick:function(clicked){
				if(this.isEditing)return;
				var context = this;
				context.currentView = context.loadItemView(clicked.id,clicked.isFolder);
				context.model.nodeId=clicked.id;
				context.model.isFolder=clicked.isFolder;
				console.log("clicked: "+clicked.label);
			},
			onExpand:function(toExpand){
				var context = this;
				this.isExpanded=!this.isExpanded;
				if(this.isEditing)return;
				treeController.loadChildrenById(toExpand.id,function(data){
					context.rebuildChildren(toExpand,data);
				});
			},
			rebuildChildren(toExpand,data){
				var context = this;
				while(toExpand.nodes.length){
					toExpand.nodes.pop();	
				}
				
				for(var i=0;i<data.length;i++){
					var child = context.buildItem(data[i]);
					toExpand.nodes.push(child);
				}
			},
			loadTree:function(rootId){
				var context = this;
				//console.log(context.model.id+"/"+context.model.action);
				treeController.loadNodeById(context.model.id,context.model.action,function(data){
					context.model.node = context.buildItem(data);
					
					
				});
			},
			buildItem:function(data){
				var context = this;
				return {
					onExpand:context.onExpand,
					onClick:context.onClick,
					blocked:function(){
						return context.isEditing;
					},
					id:data.Id,
					label:data.Title,
					isFolder:data.IsFolder,
					nodes:[]
				}
			}	
		}
	}
</script>